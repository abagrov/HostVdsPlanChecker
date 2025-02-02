using HostVdsPlan;
using Humanizer;
using NLog;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

const string envPrefix = "HOSTVDSPLAN";

long chatId = GetEnvironmentInt($"{envPrefix}_CHAT_ID");
string token = GetEnvironment($"{envPrefix}_TG_TOKEN");
string region = GetEnvironment($"{envPrefix}_REGION", "eu-north1");
var delaySeconds = (int)GetEnvironmentInt($"{envPrefix}_CHECK_EVERY_N_SECONDS", 30);
if (delaySeconds < 30) throw new ArgumentException("Delay should at least 30 seconds.");
string[] planNames = GetEnvironment($"{envPrefix}_TARGET_PLANS", "hostvds-high-frequency").Split(',', StringSplitOptions.RemoveEmptyEntries);
if (planNames.Length == 0) throw new ArgumentException($"You should provide at least one plan name via environment variable '{envPrefix}_TARGET_PLANS'.");

LogManager.Setup().LoadConfigurationFromFile();
LogManager.Configuration.Variables["telegramBotToken"] = token;
LogManager.Configuration.Variables["telegramChatId"] = chatId.ToString();

var logger = LogManager.GetCurrentClassLogger();
var startTime = DateTime.Now;
string responseBody = "";
DateTime lastCheck = DateTime.MinValue;

logger.Info($"Start {startTime}");
try
{
    SetupTg(logger, startTime);
    using HttpClient client = new();
    while (true)
    {
        await CheckVps(logger, client);
        await Task.Delay(delaySeconds * 1000);
    }
}
catch (Exception ex)
{
    logger.Fatal(ex, "Unrecoverable error");
    throw;
}


async Task CheckVps(Logger logger, HttpClient client)
{
    try
    {
        var response = await client.GetAsync($"https://hostvds.com/api/plans/?region={region}");
        responseBody = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();

        var data = JsonSerializer.Deserialize<Plan[]>(responseBody) ?? throw new ArgumentException("Deserialized JSON was null.");
        var hfAvailiable = data.Where(c => planNames.Any(plan => c.Name.Contains(plan, StringComparison.InvariantCultureIgnoreCase)) && !c.IsOutOfStock);
        lastCheck = DateTime.Now;
        if (!hfAvailiable.Any()) return;

        logger.Info($"🔥Availiable {region}! {string.Join(", ", hfAvailiable.Select(c => c.Name))}");
    }
    catch (Exception ex)
    {
        logger.Error(ex, "Error get /api/plans");
        throw;
    }
}


void SetupTg(Logger logger, DateTime startTime)
{
    var botClient = new TelegramBotClient(token);
    var receiverOptions = new ReceiverOptions
    {
        AllowedUpdates =
        [
            UpdateType.Message,
        ],
        DropPendingUpdates = true,
    };

    botClient.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions, default); // Запускаем бота

    Task ErrorHandler(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
    {
        logger.Error(exception, $"TG error");
        return Task.CompletedTask;
    }

    Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
    {
        if (update.Message?.Chat?.Id != chatId) return Task.CompletedTask;
        logger.Info("Uptime {time}, last check {checkTime}, last response {resp}", (DateTime.Now - startTime).Humanize(), lastCheck.Humanize(), responseBody);
        return Task.CompletedTask;
    }
}

static long GetEnvironmentInt(string name, long? defaultValue = null)
{
    const string dummy = "dummy";
    var s = GetEnvironment(name, dummy);
    if (s.Equals(dummy))
    {
        if (defaultValue.HasValue) return defaultValue.Value;
        throw new ArgumentException($"Environment variable '{name}' is not set.");
    }

    if (!long.TryParse(s, out var sInt)) throw new ArgumentException($"Environment variable '{name}' value '{s}' is not a valid number.");

    return sInt;
}

static string GetEnvironment(string name, string? defaultValue = null)
{
    var s = Environment.GetEnvironmentVariable(name);
    if (string.IsNullOrEmpty(s))
    {
        if (!string.IsNullOrEmpty(defaultValue)) return defaultValue;

        throw new ArgumentException($"Environment variable '{name}' is not set.");
    }

    return s;
}