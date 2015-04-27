using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace thestory.DataModel
{
    public class StoryItem
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "isroot")]
        public bool IsRoot { get; set; }

        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }

        [JsonProperty(PropertyName = "popularity")]
        public int Popularity { get; set; }

        [JsonProperty(PropertyName = "choice")]
        public string Choice { get; set; }

        [JsonIgnore]
        public List<Choice> Choices { get; set; }
    }
}
