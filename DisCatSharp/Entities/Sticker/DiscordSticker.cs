// This file is part of the DisCatSharp project.
//
// Copyright (c) 2021 AITSYS
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DisCatSharp.Exceptions;
using DisCatSharp.Enums.Discord;
using Newtonsoft.Json;
using DisCatSharp.Net;

namespace DisCatSharp.Entities
{
    /// <summary>
    /// Represents a Discord Sticker.
    /// </summary>
    public class DiscordSticker : SnowflakeObject, IEquatable<DiscordSticker>
    {
        /// <summary>
        /// Gets the Pack ID of this sticker.
        /// </summary>
        [JsonProperty("pack_id")]
        public ulong PackId { get; internal set; }

        /// <summary>
        /// Gets the Name of the sticker.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the Description of the sticker.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; internal set; }

        /// <summary>
        /// Gets the type of sticker.
        /// </summary>
        [JsonProperty("type")]
        public StickerType Type { get; internal set; }

        /// <summary>
        /// For guild stickers, gets the user that made the sticker.
        /// </summary>
        [JsonProperty("user")]
        public DiscordUser User { get; internal set; }

        /// <summary>
        /// Gets the guild associated with this sticker, if any.
        /// </summary>
        public DiscordGuild Guild => (this.Discord as DiscordClient).InternalGetCachedGuild(this.GuildId);

        /// <summary>
        /// Gets the guild id the sticker belongs too.
        /// </summary>
        [JsonProperty("guild_id")]
        public ulong? GuildId { get; internal set; }

        /// <summary>
        /// Gets whether this sticker is available. Only applicable to guild stickers.
        /// </summary>
        [JsonProperty("available")]
        public bool Available { get; internal set; }

        /// <summary>
        /// Gets the sticker's sort order, if it's in a pack.
        /// </summary>
        [JsonProperty("sort_value")]
        public int SortValue { get; internal set; }

        /// <summary>
        /// Gets the list of tags for the sticker.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<string> Tags
            => this._internalTags != null ? this._internalTags.Split(',') : Array.Empty<string>();

        /// <summary>
        /// Gets the asset hash of the sticker.
        /// </summary>
        [JsonProperty("asset")]
        public string Asset { get; internal set; }

        /// <summary>
        /// Gets the preview asset hash of the sticker.
        /// </summary>
        [JsonProperty("preview_asset", NullValueHandling = NullValueHandling.Ignore)]
        public string PreviewAsset { get; internal set; }

        /// <summary>
        /// Gets the Format type of the sticker.
        /// </summary>
        [JsonProperty("format_type")]
        public StickerFormat FormatType { get; internal set; }

        /// <summary>
        /// Gets the tags of the sticker.
        /// </summary>
        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
        internal string _internalTags { get; set; }

        /// <summary>
        /// Gets the url of the sticker.
        /// </summary>
        [JsonIgnore]
        public string Url => $"{DiscordDomain.GetDomain(CoreDomain.DiscordCdn).Url}{Endpoints.STICKERS}/{this.Id}.png";

        /// <summary>
        /// Whether to stickers are equal.
        /// </summary>
        /// <param name="other">DiscordSticker</param>
        /// <returns></returns>
        public bool Equals(DiscordSticker other) => this.Id == other.Id;

        /// <summary>
        /// Gets the sticker in readable format.
        /// </summary>
        public override string ToString() => $"Sticker {this.Id}; {this.Name}; {this.FormatType}";

        /// <summary>
        /// Modifies the sticker
        /// </summary>
        /// <param name="name">The name of the sticker</param>
        /// <param name="description">The description of the sticker</param>
        /// <param name="tags">The name of a unicode emoji representing the sticker's expression</param>
        /// <param name="reason">Audit log reason</param>
        /// <returns>A sticker object</returns>
        /// <exception cref="UnauthorizedException">Thrown when the sticker could not be found.</exception>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageEmojisAndStickers"/> permission.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        /// <exception cref="ArgumentException">Sticker does not belong to a guild.</exception>
        public Task<DiscordSticker> ModifyAsync(Optional<string> name, Optional<string> description, Optional<string> tags, string reason = null)
        {
            return !this.GuildId.HasValue
                ? throw new ArgumentException("This sticker does not belong to a guild.")
                : name.HasValue && (name.Value.Length < 2 || name.Value.Length > 30)
                ? throw new ArgumentException("Sticker name needs to be between 2 and 30 characters long.")
                : description.HasValue && (description.Value.Length < 1 || description.Value.Length > 100)
                ? throw new ArgumentException("Sticker description needs to be between 1 and 100 characters long.")
                : tags.HasValue && !DiscordEmoji.TryFromUnicode(this.Discord, tags.Value, out var emoji)
                ? throw new ArgumentException("Sticker tags needs to be a unicode emoji.")
                : this.Discord.ApiClient.ModifyGuildStickerAsync(this.GuildId.Value, this.Id, name, description, tags, reason);
        }

        /// <summary>
        /// Deletes the sticker
        /// </summary>
        /// <param name="reason">Audit log reason</param>
        /// <exception cref="UnauthorizedException">Thrown when the sticker could not be found.</exception>
        /// <exception cref="UnauthorizedException">Thrown when the client does not have the <see cref="Permissions.ManageEmojisAndStickers"/> permission.</exception>
        /// <exception cref="ServerErrorException">Thrown when Discord is unable to process the request.</exception>
        /// <exception cref="ArgumentException">Sticker does not belong to a guild.</exception>
        public Task DeleteAsync(string reason = null)
            => this.GuildId.HasValue ? this.Discord.ApiClient.DeleteGuildStickerAsync(this.GuildId.Value, this.Id, reason) : throw new ArgumentException("The requested sticker is no guild sticker.");
    }

    /// <summary>
    /// The sticker type
    /// </summary>
    public enum StickerType : long
    {
        /// <summary>
        /// Standard nitro sticker
        /// </summary>
        Standard = 1,
        /// <summary>
        /// Custom guild sticker
        /// </summary>
        Guild = 2
    }

    /// <summary>
    /// The sticker type
    /// </summary>
    public enum StickerFormat : long
    {
        /// <summary>
        /// Sticker is a png
        /// </summary>
        PNG = 1,
        /// <summary>
        /// Sticker is a animated png
        /// </summary>
        APNG = 2,
        /// <summary>
        /// Sticker is lottie
        /// </summary>
        LOTTIE = 3
    }
}
