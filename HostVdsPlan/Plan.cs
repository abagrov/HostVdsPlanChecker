using System.Text.Json.Serialization;

namespace HostVdsPlan
{
    public class Plan
    {
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }

        [JsonPropertyName("hourly")]
        public double Hourly { get; set; }

        [JsonPropertyName("hourly_promo_striked")]
        public double HourlyPromoStriked { get; set; }

        [JsonPropertyName("disk")]
        public int Disk { get; set; }

        [JsonPropertyName("ram")]
        public int Ram { get; set; }

        [JsonPropertyName("vcpus")]
        public int Vcpus { get; set; }

        [JsonPropertyName("is_out_of_stock")]
        public bool IsOutOfStock { get; set; }

        [JsonPropertyName("is_low_stock")]
        public bool IsLowStock { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("flavor_uuid")]
        public string FlavorUuid { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("note")]
        public object Note { get; set; }

        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("monthly")]
        public double Monthly { get; set; }

        [JsonPropertyName("monthly_promo_striked")]
        public double MonthlyPromoStriked { get; set; }

        [JsonPropertyName("force_out_of_stock")]
        public bool ForceOutOfStock { get; set; }

        [JsonPropertyName("require_monthly_coverage")]
        public bool RequireMonthlyCoverage { get; set; }

        [JsonPropertyName("main")]
        public bool Main { get; set; }

        [JsonPropertyName("traffic")]
        public int Traffic { get; set; }

        [JsonPropertyName("bandwidth")]
        public int Bandwidth { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("customer_uuid")]
        public object CustomerUuid { get; set; }
    }


}
