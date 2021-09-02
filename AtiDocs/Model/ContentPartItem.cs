using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Timers;

namespace AtiDocs.Model
{
    public class ContentPartItem
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonIgnore]
        public string ContentHTML { get; set; }

        [JsonIgnore]
        public bool Edit { get; set; } = true;

        [JsonIgnore]
        public string ElementReference { get; set; }

        [JsonIgnore]
        public System.Timers.Timer FocusTimer { get; set; }
    }
}
