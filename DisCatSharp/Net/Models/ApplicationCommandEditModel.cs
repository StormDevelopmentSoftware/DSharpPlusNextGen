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
using DisCatSharp.Entities;

namespace DisCatSharp.Net.Models
{
    /// <summary>
    /// Represents a application command edit model.
    /// </summary>
    public class ApplicationCommandEditModel
    {
        /// <summary>
        /// Sets the command's new name.
        /// </summary>
        public Optional<string> Name
        {
            internal get => this._name;
            set
            {
                if (value.Value.Length > 32)
                    throw new ArgumentException("Slash command name cannot exceed 32 characters.", nameof(value));
                this._name = value;
            }
        }
        private Optional<string> _name;

        /// <summary>
        /// Sets the command's new description
        /// </summary>
        public Optional<string> Description
        {
            internal get => this._description;
            set
            {
                if (value.Value.Length > 100)
                    throw new ArgumentException("Slash command description cannot exceed 100 characters.", nameof(value));
                this._description = value;
            }
        }
        private Optional<string> _description;

        /// <summary>
        /// Sets the command's new options.
        /// </summary>
        public Optional<IReadOnlyCollection<DiscordApplicationCommandOption>> Options { internal get; set; }

        /// <summary>
        /// Sets the command's default permission.
        /// </summary>
        public Optional<bool> DefaultPermission { internal get; set; }
    }
}
