namespace Unosquare.FFME.Windows.Sample.Foundation
{
    using Playlists;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    /// <summary>
    /// A custom playlist entry with notification properties backed by Attributes.
    /// </summary>
    /// <seealso cref="PlaylistEntry" />
    public sealed class CustomPlaylistEntry : PlaylistEntry
    {
        private static readonly Dictionary<string, string> PropertyMap = new Dictionary<string, string>
        {
            { nameof(Thumbnail), "ffme-thumbnail" },
            { nameof(Format), "info-format" },
            { nameof(LastOpenedUtc), "ffme-lastopened" },
            { nameof(NoiseTimeLine), "noise-timeline" }
        };

        /// <summary>
        /// Gets or sets the thumbnail.
        /// </summary>
        public string Thumbnail
        {
            get => GetMappedAttributeValue();
            set => SetMappedAttributeValue(value);
        }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        public string Format
        {
            get => GetMappedAttributeValue();
            set => SetMappedAttributeValue(value);
        }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        public TimeLine NoiseTimeLine
        {
            get
            {
                var currentValue = GetMappedAttributeValue();
                if (string.IsNullOrEmpty(currentValue)) { 
                    return null;
                }
                var tl = new TimeLine();
                var split = currentValue.Split('|');


                tl.Duration = TimeSpan.FromTicks(Convert.ToInt64(split.First()));
                foreach(var eve in split.Last().Split(';'))
                {
                    if (string.IsNullOrWhiteSpace(eve)) continue;
                    var kvp = eve.Split(':');
                    tl.Events.Add(new TimeLineEvent { Start = TimeSpan.FromTicks(Convert.ToInt64(kvp.First())), Duration = TimeSpan.FromTicks(Convert.ToInt64(kvp.Last())) });
                }
                return tl;
            }
            set
            {
                if (value != null)
                {
                    var sb = new StringBuilder();
                    sb.Append($"{value.Duration.Ticks}|");

                    foreach (var eve in value.Events)
                        sb.Append($"{eve.Start.Ticks}:{eve.Duration.Ticks};");

                    SetMappedAttributeValue(sb.ToString());
                }
            }
        }

        /// <summary>
        /// Gets or sets the last opened UTC.
        /// </summary>
        public DateTime? LastOpenedUtc
        {
            get
            {
                var currentValue = GetMappedAttributeValue();
                if (string.IsNullOrWhiteSpace(currentValue))
                    return default;

                return long.TryParse(currentValue, out var binaryValue) ?
                    DateTime.FromBinary(binaryValue) :
                    default(DateTime?);
            }
            set
            {
                if (value == null)
                {
                    SetMappedAttributeValue(null);
                    return;
                }

                var binaryValue = value.Value.ToBinary().ToString(CultureInfo.InvariantCulture);
                SetMappedAttributeValue(binaryValue);
            }
        }

        private string GetMappedAttributeValue([CallerMemberName] string propertyName = null) =>
            Attributes.GetEntryValue(PropertyMap[propertyName ?? throw new ArgumentNullException(nameof(propertyName))]);

        private void SetMappedAttributeValue(string value, [CallerMemberName] string propertyName = null)
        {
            if (Attributes.SetEntryValue(PropertyMap[propertyName ?? throw new ArgumentNullException(nameof(propertyName))], value))
                OnPropertyChanged(propertyName);
        }
    }
}
