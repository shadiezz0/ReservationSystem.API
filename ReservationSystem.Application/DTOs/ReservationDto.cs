using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace ReservationSystem.Application.DTOs
{
    // Json converter to serialize TimeSpan as "HH:mm"
    public class TimeSpanHoursMinutesJsonConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s))
                return default;

            if (TimeSpan.TryParseExact(s, "hh\\:mm", CultureInfo.InvariantCulture, out var ts))
                return ts;

            if (TimeSpan.TryParse(s, out ts))
                return ts;

            throw new JsonException("Invalid time format. Expected 'HH:mm'.");
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("hh\\:mm"));
        }
    }

    public class CreateReservationDto
    {
        public DateTime ReservationDate { get; set; }

        [JsonConverter(typeof(TimeSpanHoursMinutesJsonConverter))]
        [DefaultValue("09:00")] // Shown in Swagger
        [Display(Name = "Start Time (HH:mm)", Description = "Time in 24h format HH:mm")]
        public TimeSpan StartTime { get; set; }

        // Show default in Swagger as HH:mm and serialize as "HH:mm"
        [JsonConverter(typeof(TimeSpanHoursMinutesJsonConverter))]
        [DefaultValue("10:00")] // Swagger default (1 hour after sample StartTime)
        [Display(Name = "End Time (HH:mm)", Description = "Time in 24h format HH:mm (must be after StartTime)")]
        public TimeSpan EndTime { get; set; }

        [DefaultValue(true)]
        public bool IsAvailable { get; set; }

        public int ItemId { get; set; }
        [JsonIgnore]
        public int ItemTypeId { get; set; }

    }

    public class UpdateReservationDto: CreateReservationDto
    {
        [JsonIgnore]
        public int Id { get; set; }
    }

    public class FilterReservationDto
    {
        // Date range filters
        public DateTime FromDate { get; set; } = DateTime.Now;
        public DateTime ToDate { get; set; } = DateTime.UtcNow;


        // Time filters
        [JsonConverter(typeof(TimeSpanHoursMinutesJsonConverter))]
        [DefaultValue("09:00")] // Shown in Swagger
        [Display(Name = "Start Time (HH:mm)", Description = "Time in 24h format HH:mm")]
        public TimeSpan? StartTime { get; set; }

        // Show default in Swagger as HH:mm and serialize as "HH:mm"
        [JsonConverter(typeof(TimeSpanHoursMinutesJsonConverter))]
        [DefaultValue("10:00")] // Swagger default (1 hour after sample StartTime)
        [Display(Name = "End Time (HH:mm)", Description = "Time in 24h format HH:mm (must be after StartTime)")]
        public TimeSpan? EndTime { get; set; }

        // Item filters
        public int ItemId { get; set; } = 0;
        public int ItemTypeId { get; set; } = 0;

        // Status filter
        public Status Status { get; set; } = 0;

        // User filter (for admin to filter by specific user)
        [JsonIgnore]
        public int UserId { get; set; }
    }

    public class FilterIsavilableReservayionDto: CreateReservationDto
    {
    }

    public class ReservationDto
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public Status Status { get; set; }
        public bool IsAvailable { get; set; } = true;
        public double TotalPrice { get; set; }
    }
}
