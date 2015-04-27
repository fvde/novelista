﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace thestory
{
    public class StoryItem
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "choice")]
        public string Choice { get; set; }

        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }

        [JsonProperty(PropertyName = "popularity")]
        public int Popularity { get; set; }
    }
}
