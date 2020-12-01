﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable enable

using System;
using System.Collections.Generic;
using osu.Game.Utils;

namespace osu.Game.Audio
{
    /// <summary>
    /// Describes a gameplay hit sample.
    /// </summary>
    [Serializable]
    public class HitSampleInfo : ISampleInfo
    {
        public const string HIT_WHISTLE = @"hitwhistle";
        public const string HIT_FINISH = @"hitfinish";
        public const string HIT_NORMAL = @"hitnormal";
        public const string HIT_CLAP = @"hitclap";

        /// <summary>
        /// All valid sample addition constants.
        /// </summary>
        public static IEnumerable<string> AllAdditions => new[] { HIT_WHISTLE, HIT_CLAP, HIT_FINISH };

        /// <summary>
        /// The name of the sample to load.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The bank to load the sample from.
        /// </summary>
        public readonly string? Bank;

        /// <summary>
        /// An optional suffix to provide priority lookup. Falls back to non-suffixed <see cref="Name"/>.
        /// </summary>
        public readonly string? Suffix;

        /// <summary>
        /// The sample volume.
        /// </summary>
        public int Volume { get; }

        public HitSampleInfo(string name, string? bank = null, string? suffix = null, int volume = 100)
        {
            Name = name;
            Bank = bank;
            Suffix = suffix;
            Volume = volume;
        }

        /// <summary>
        /// Retrieve all possible filenames that can be used as a source, returned in order of preference (highest first).
        /// </summary>
        public virtual IEnumerable<string> LookupNames
        {
            get
            {
                if (!string.IsNullOrEmpty(Suffix))
                    yield return $"Gameplay/{Bank}-{Name}{Suffix}";

                yield return $"Gameplay/{Bank}-{Name}";
            }
        }

        /// <summary>
        /// Creates a new <see cref="HitSampleInfo"/> with overridden values.
        /// </summary>
        /// <param name="name">An optional new sample name.</param>
        /// <param name="bank">An optional new sample bank.</param>
        /// <param name="suffix">An optional new lookup suffix.</param>
        /// <param name="volume">An optional new volume.</param>
        /// <returns>The new <see cref="HitSampleInfo"/>.</returns>
        public virtual HitSampleInfo With(Optional<string> name = default, Optional<string?> bank = default, Optional<string?> suffix = default, Optional<int> volume = default)
            => new HitSampleInfo(name.GetOr(Name), bank.GetOr(Bank), suffix.GetOr(Suffix), volume.GetOr(Volume));
    }
}
