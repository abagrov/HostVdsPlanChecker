### HOSTVDS.COM available plans checker

Checks available HOSTVDS plans (servers) and 
writes a message to Telegram group chat if target plans are available.

### How to use

1. Create Telegram bot with @botfather
2. Create Telegram group chat, add bot to this chat
3. Get Telegram group chat id - navigate to https://api.telegram.org/bot<YOUR_BOT_TOKEN>/getUpdates, there should be message with chat id where you added your bot. If not, write a message to group chat and check again.
3. Run 
```
sudo docker run -d -e HOSTVDSPLAN_CHAT_ID='<CHAT ID WITH YOUR BOT>' \
	-e HOSTVDSPLAN_TG_TOKEN='<TELEGRAM TOKEN OF YOUR BOT>' \
	--restart=unless-stopped \
	--name hostvds-plan-checker \
	abagrov/hostvds-plan-checker:0.1
```
4. When target plans are available, bot will send message to group chat each N seconds, so you wont miss your desired server🔥.
5. To check if the bot is alive, write any message to group chat - bot will respond with uptime and last answer from API.

### Environment variables

| Env variable name                 | Default value          | Description                                                                                          |
|-----------------------------------|------------------------|------------------------------------------------------------------------------------------------------|
| `HOSTVDSPLAN_TG_TOKEN`              | N/A                    | Telegram bot token                                                                                   |
| `HOSTVDSPLAN_CHAT_ID`               | N/A                    | Chat id, where you added your bot                                                                    |
| `HOSTVDSPLAN_REGION`                | eu-north1              | Region where to check plans. Currently available: https://hostvds.com/api/regions/           |
| `HOSTVDSPLAN_CHECK_EVERY_N_SECONDS` | 30                     | Check available plans every N seconds. Minimum 30 sec                                                               |
| `HOSTVDSPLAN_TARGET_PLANS`          | high-frequency | Desired plan names, delimited with commas. If at least one plan is available, bot will inform. See: https://hostvds.com/api/plans/?region=eu-north1 |


