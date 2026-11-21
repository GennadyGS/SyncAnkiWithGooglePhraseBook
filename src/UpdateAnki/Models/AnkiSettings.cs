using System.Collections.ObjectModel;

namespace UpdateAnki.Models;

internal sealed class AnkiSettings(IReadOnlyList<AnkiDeckSettings> settings)
    : ReadOnlyCollection<AnkiDeckSettings>(settings.ToList());
