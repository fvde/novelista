using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace thestory.DataModel
{
    public class Choice
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "ancestorId")]
        public string AncestorId { get; set; }

        [JsonProperty(PropertyName = "choiceId")]
        public string ChoiceId { get; set; }
    }
}
