using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;

// rozstawić this-y

namespace OOP_project.Data
{
    public class Database
    {
        private readonly string _connectionString =
            @"Data Source=C:\LinguaForge\linguaForge.db";

        public SqliteConnection GetConnection()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }

        public Dictionary<string, string> ReadSymbols()
        {
            var symbols = new Dictionary<string, string>();

            using var connection = GetConnection();
            using var command = new SqliteCommand(
                "SELECT Symbol, Category FROM AlphabetLibrary", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string symbol = reader.GetString(0);
                string category = reader.GetString(1);

                symbols[symbol] = category;
            }

            return symbols;
        }

        public Dictionary<string, string> ReadAlphabet()
        {
            var symbols = new Dictionary<string, string>();

            using var connection = GetConnection();
            using var command = new SqliteCommand(
                "SELECT Code, Category FROM AlphabetSymbols", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string symbol = reader.GetString(0);
                string category = reader.GetString(1);

                symbols[symbol] = category;
            }

            return symbols;
        }


        public List<string> ReadGramaticalRoles()
        {
            var codes = new List<string>();

            using var connection = GetConnection();
            using var command = new SqliteCommand(
                "SELECT Code FROM GrammaticalLibrary", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string code = reader.GetString(0);

                codes.Append(code);
            }

            return codes ;
        }

        //public List<string> ReadAffixes()
        //{
        //    var items = new List<string>();

        //    using var connection = GetConnection();
        //    using var command = new SqliteCommand(
        //        "SELECT Affix FROM Affixes", connection);
        //    using var reader = command.ExecuteReader();

        //    while (reader.Read())
        //    {
        //        string item = reader.GetString(0);

        //        items.Append(item);
        //    }

        //    return items;
        //}

        //public List<string> ReadAffixesType()
        //{
        //    var items = new List<string>();

        //    using var connection = GetConnection();

        //    using var command = new SqliteCommand(
        //        @"SELECT AffixType.Name
        //  FROM Affixes
        //  INNER JOIN AffixType
        //      ON Affixes.AffixTypeId = AffixType.Id",
        //        connection);

        //    using var reader = command.ExecuteReader();

        //    while (reader.Read())
        //    {
        //        string item = reader.GetString(0);
        //        items.Add(item);
        //    }

        //    return items;
        //}

        public List<(string Affix, string Type)> ReadAffixesWithTypes()
        {
            var items = new List<(string Affix, string Type)>();

            using var connection = GetConnection();

            using var command = new SqliteCommand(
                @"SELECT Affixes.Affix, AffixType.Name
          FROM Affixes
          INNER JOIN AffixType
              ON Affixes.AffixTypeId = AffixType.Id",
                connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                items.Add((
                    reader.GetString(0),
                    reader.GetString(1)
                ));
            }

            return items;
        }

        public List<string> ReadRoots()
        {
            var items = new List<string>();

            using var connection = GetConnection();
            using var command = new SqliteCommand(
                "SELECT Root FROM Roots", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string item = reader.GetString(0);

                items.Append(item);
            }

            return items;
        }

 // sprawdzić czy Id się nie będą nakładać

        public void InitializeDatabase()
        {
            using var connection = GetConnection();

            string sql = @"
        CREATE TABLE IF NOT EXISTS AlphabetLibrary (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Symbol TEXT NOT NULL,
            Category TEXT NOT NULL
        );

            CREATE TABLE IF NOT EXISTS Affixes (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Affix TEXT NOT NULL,
            PartOfSpeechId INTEGER NOT NULL,
            GrammaticalCategoryId INTEGER NOT NULL,
            AffixTypeId INTEGER NOT NULL,
            FOREIGN KEY (PartOfSpeechId) REFERENCES PartOfSpeech(Id),
            FOREIGN KEY (GrammaticalCategoryId) REFERENCES GrammaticalLibrary (Code),
            FOREIGN KEY (AffixTypeId) REFERENCES AffixType(Id)
        );

            CREATE TABLE IF NOT EXISTS AffixAllowedRoots (
            AffixId INTEGER NOT NULL,
            RootId INTEGER NOT NULL,

            PRIMARY KEY (AffixId, RootId),

            FOREIGN KEY (AffixId) REFERENCES Affixes(Id),
            FOREIGN KEY (RootId) REFERENCES Roots(Id)
        );

            CREATE TABLE IF NOT EXISTS AlphabetSymbols (
                Id         INTEGER PRIMARY KEY AUTOINCREMENT,
                LanguageId INTEGER NOT NULL,
                Symbol     TEXT    NOT NULL,
                Category   TEXT    NOT NULL,
                FOREIGN KEY (
                    LanguageId
                )
                REFERENCES Languages (Id) 
            );

            CREATE TABLE IF NOT EXISTS Roots (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Root TEXT NOT NULL,
                TranslateId TEXT,
            );

            CREATE TABLE IF NOT EXISTS GrammaticalCategory (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                PartOfSpeechId INTEGER NOT NULL,
                Description TEXT,
                FOREIGN KEY (PartOfSpeechId)
                    REFERENCES PartOfSpeech(Id),
                UNIQUE (Name, PartOfSpeechId)
            );

            CREATE TABLE IF NOT EXISTS AffixType (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE
            );

            INSERT OR IGNORE INTO AffixType (Name)
            VALUES
                ('Prefix'),
                ('Suffix'),
                ('Infix'),
                ('Circumfix'),
                ('Suprafix'),
                ('Interfix'),
                ('Transfix');

            CREATE TABLE IF NOT EXISTS PartOfSpeech (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE,
                Description TEXT
            );

            INSERT OR IGNORE INTO PartOfSpeech (Name, Description)
            VALUES
                ('Noun', 'Rzeczownik'),
                ('Verb', 'Czasownik'),
                ('Adjective', 'Przymiotnik'),
                ('Adverb', 'Przysłówek'),
                ('Pronoun', 'Zaimek'),
                ('Numeral', 'Liczebnik'),
                ('Preposition', 'Przyimek'),
                ('Postposition', 'Poimek'),
                ('Conjunction', 'Spójnik'),
                ('Interjection', 'Wykrzyknik'),
                ('Particle', 'Partykuła'),
                ('Determiner', 'Określnik'),
                ('Article', 'Rodzajnik'),
                ('AuxiliaryVerb', 'Czasownik posiłkowy'),
                ('ModalVerb', 'Czasownik modalny'),
                ('Copula', 'Łącznik'),
                ('Classifier', 'Klasyfikator'),
                ('Counter', 'Wyraz klasyfikujący'),
                ('Quantifier', 'Kwantyfikator'),
                ('Possessive', 'Wyraz dzierżawczy'),
                ('Demonstrative', 'Wyraz wskazujący'),
                ('RelativePronoun', 'Zaimek względny'),
                ('InterrogativePronoun', 'Zaimek pytajny'),
                ('ReflexivePronoun', 'Zaimek zwrotny'),
                ('ReciprocalPronoun', 'Zaimek wzajemny'),
                ('PersonalPronoun', 'Zaimek osobowy'),
                ('IndefinitePronoun', 'Zaimek nieokreślony'),
                ('NegativePronoun', 'Zaimek przeczący'),
                ('Converb', 'Forma czasownikowa łącząca czynności'),
                ('Participle', 'Imiesłów'),
                ('Infinitive', 'Bezokolicznik'),
                ('Gerund', 'Gerundium'),
                ('VerbalNoun', 'Rzeczownik odczasownikowy'),
                ('AuxiliaryParticle', 'Partykuła pomocnicza'),
                ('FocusParticle', 'Partykuła wyróżniająca'),
                ('NegationParticle', 'Partykuła przecząca'),
                ('DiscourseParticle', 'Partykuła dyskursywna'),
                ('Clitic', 'Klit yka'),
                ('Onomatopoeia', 'Wyraz dźwiękonaśladowczy'),
                ('Ideophone', 'Ideofon'),
                ('Abbreviation', 'Skrótowiec'),
                ('Symbol', 'Symbol'),
                ('Greeting', 'Forma powitalna'),
                ('Farewell', 'Forma pożegnalna'),
                ('Affirmation', 'Wyraz potwierdzający'),
                ('Negation', 'Wyraz przeczący'),
                ('Filler', 'Wypełniacz wypowiedzi');

                CREATE TABLE IF NOT EXISTS GrammaticalLibrary (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Category TEXT NOT NULL,
                    Code TEXT NOT NULL UNIQUE,
                    Description TEXT NOT NULL
                );

                INSERT OR IGNORE INTO GrammaticalLibrary
                    (Category, Code, Description)
                VALUES
                -- Number
                ('Number', 'Singular', 'Liczba pojedyncza'),
                ('Number', 'Plural', 'Liczba mnoga'),
                ('Number', 'Dual', 'Liczba podwójna'),
                ('Number', 'Trial', 'Liczba potrójna'),
                ('Number', 'Paucal', 'Liczba niewielka'),
                ('Number', 'Collective', 'Liczba zbiorowa'),

                -- Person
                ('Person', 'FirstPerson', 'Pierwsza osoba'),
                ('Person', 'SecondPerson', 'Druga osoba'),
                ('Person', 'ThirdPerson', 'Trzecia osoba'),
                ('Person', 'Inclusive', 'Osoba włączająca rozmówcę'),
                ('Person', 'Exclusive', 'Osoba wyłączająca rozmówcę'),

                -- Gender
                ('Gender', 'Masculine', 'Rodzaj męski'),
                ('Gender', 'Feminine', 'Rodzaj żeński'),
                ('Gender', 'Neuter', 'Rodzaj nijaki'),
                ('Gender', 'CommonGender', 'Rodzaj wspólny'),
                ('Gender', 'Animate', 'Rodzaj żywotny'),
                ('Gender', 'Inanimate', 'Rodzaj nieżywotny'),

                -- Case
                ('Case', 'Nominative', 'Mianownik'),
                ('Case', 'Accusative', 'Biernik'),
                ('Case', 'Genitive', 'Dopełniacz'),
                ('Case', 'Dative', 'Celownik'),
                ('Case', 'Instrumental', 'Narzędnik'),
                ('Case', 'Locative', 'Miejscownik'),
                ('Case', 'Vocative', 'Wołacz'),
                ('Case', 'Ablative', 'Ablatyw'),
                ('Case', 'Allative', 'Allatyw'),
                ('Case', 'Elative', 'Elatyw'),
                ('Case', 'Illative', 'Ilatiw'),
                ('Case', 'Adessive', 'Adesyw'),
                ('Case', 'Inessive', 'Inesyw'),
                ('Case', 'Essive', 'Esyw'),
                ('Case', 'Translative', 'Translatyw'),
                ('Case', 'Comitative', 'Komitatyw'),
                ('Case', 'Benefactive', 'Benefaktywny przypadek'),
                ('Case', 'Partitive', 'Partytyw'),

                -- Tense
                ('Tense', 'Present', 'Czas teraźniejszy'),
                ('Tense', 'Past', 'Czas przeszły'),
                ('Tense', 'Future', 'Czas przyszły'),
                ('Tense', 'RemotePast', 'Czas odległej przeszłości'),
                ('Tense', 'RecentPast', 'Czas niedawnej przeszłości'),
                ('Tense', 'ImmediateFuture', 'Czas najbliższej przyszłości'),

                -- Aspect
                ('Aspect', 'Perfective', 'Aspekt dokonany'),
                ('Aspect', 'Imperfective', 'Aspekt niedokonany'),
                ('Aspect', 'Progressive', 'Aspekt ciągły'),
                ('Aspect', 'Habitual', 'Aspekt nawykowy'),
                ('Aspect', 'Iterative', 'Aspekt iteratywny'),
                ('Aspect', 'Inceptive', 'Aspekt początkowy'),
                ('Aspect', 'Terminative', 'Aspekt końcowy'),
                ('Aspect', 'Perfect', 'Aspekt rezultatywny'),

                -- Mood
                ('Mood', 'Indicative', 'Tryb oznajmujący'),
                ('Mood', 'Imperative', 'Tryb rozkazujący'),
                ('Mood', 'Conditional', 'Tryb warunkowy'),
                ('Mood', 'Subjunctive', 'Tryb łączący'),
                ('Mood', 'Optative', 'Tryb życzący'),
                ('Mood', 'Potential', 'Tryb potencjalny'),
                ('Mood', 'Jussive', 'Tryb życząco-rozkazujący'),
                ('Mood', 'Dubitative', 'Tryb wątpiący'),

                -- Voice
                ('Voice', 'Active', 'Strona czynna'),
                ('Voice', 'Passive', 'Strona bierna'),
                ('Voice', 'Middle', 'Strona medialna'),
                ('Voice', 'Reflexive', 'Strona zwrotna'),
                ('Voice', 'Causative', 'Konstrukcja sprawcza'),

                -- Degree
                ('Degree', 'Positive', 'Stopień równy'),
                ('Degree', 'Comparative', 'Stopień wyższy'),
                ('Degree', 'Superlative', 'Stopień najwyższy'),
                ('Degree', 'Equative', 'Stopień równoważny'),
                ('Degree', 'Excessive', 'Stopień nadmierny'),

                -- Polarity
                ('Polarity', 'Positive', 'Twierdzenie'),
                ('Polarity', 'Negative', 'Przeczenie'),
                ('Polarity', 'DoubleNegative', 'Podwójne przeczenie'),

                -- Definiteness
                ('Definiteness', 'Definite', 'Określoność'),
                ('Definiteness', 'Indefinite', 'Nieokreśloność'),
                ('Definiteness', 'Specific', 'Konkretność'),
                ('Definiteness', 'NonSpecific', 'Brak konkretnego odniesienia'),

                -- Numeral and quantity
                ('Quantity', 'Distributive', 'Znaczenie rozdzielcze'),
                ('Quantity', 'Multiplicative', 'Znaczenie wielokrotności'),
                ('Quantity', 'Fractional', 'Znaczenie ułamkowe'),
                ('Quantity', 'Ordinal', 'Liczebnik porządkowy'),
                ('Quantity', 'Cardinal', 'Liczebnik główny'),

                -- Derivation
                ('Derivation', 'AgentNoun', 'Rzeczownik oznaczający wykonawcę czynności'),
                ('Derivation', 'ActionNoun', 'Rzeczownik oznaczający czynność'),
                ('Derivation', 'ResultNoun', 'Rzeczownik oznaczający rezultat'),
                ('Derivation', 'InstrumentNoun', 'Rzeczownik oznaczający narzędzie'),
                ('Derivation', 'PlaceNoun', 'Rzeczownik oznaczający miejsce'),
                ('Derivation', 'Diminutive', 'Znaczenie zdrobniałe'),
                ('Derivation', 'Augmentative', 'Znaczenie zgrubiałe'),
                ('Derivation', 'Pejorative', 'Znaczenie pejoratywne'),
                ('Derivation', 'Endearment', 'Znaczenie pieszczotliwe'),
                ('Derivation', 'Nominalizer', 'Tworzenie rzeczownika'),
                ('Derivation', 'Verbalizer', 'Tworzenie czasownika'),
                ('Derivation', 'Adjectivizer', 'Tworzenie przymiotnika'),
                ('Derivation', 'Adverbializer', 'Tworzenie przysłówka'),
                ('Derivation', 'AbstractNoun', 'Tworzenie rzeczownika abstrakcyjnego'),
                ('Derivation', 'Patronymic', 'Tworzenie imienia odojcowskiego'),
                ('Derivation', 'FeminineForm', 'Tworzenie formy żeńskiej'),
                ('Derivation', 'MasculineForm', 'Tworzenie formy męskiej'),

                -- Modality
                ('Modality', 'Ability', 'Zdolność'),
                ('Modality', 'Possibility', 'Możliwość'),
                ('Modality', 'Necessity', 'Konieczność'),
                ('Modality', 'Obligation', 'Obowiązek'),
                ('Modality', 'Permission', 'Pozwolenie'),
                ('Modality', 'Prohibition', 'Zakaz'),
                ('Modality', 'Intention', 'Zamiar'),
                ('Modality', 'Desire', 'Pragnienie'),
                ('Modality', 'Volition', 'Wola'),
                ('Modality', 'Expectation', 'Oczekiwanie'),

                -- Evidentiality
                ('Evidentiality', 'DirectEvidence', 'Bezpośrednie świadectwo'),
                ('Evidentiality', 'VisualEvidence', 'Świadectwo wzrokowe'),
                ('Evidentiality', 'AuditoryEvidence', 'Świadectwo słuchowe'),
                ('Evidentiality', 'ReportedEvidence', 'Informacja z relacji innych osób'),
                ('Evidentiality', 'InferentialEvidence', 'Wniosek wyprowadzony z przesłanek'),
                ('Evidentiality', 'AssumedEvidence', 'Informacja oparta na założeniu'),

                -- Information structure
                ('InformationStructure', 'Topic', 'Temat wypowiedzi'),
                ('InformationStructure', 'Focus', 'Element wyróżniony w wypowiedzi'),
                ('InformationStructure', 'ContrastiveFocus', 'Fokus kontrastywny'),
                ('InformationStructure', 'Emphasis', 'Wzmocnienie znaczenia'),
                ('InformationStructure', 'Restrictive', 'Znaczenie ograniczające'),

                -- Agreement
                ('Agreement', 'SubjectAgreement', 'Zgodność z podmiotem'),
                ('Agreement', 'ObjectAgreement', 'Zgodność z dopełnieniem'),
                ('Agreement', 'GenderAgreement', 'Zgodność rodzaju'),
                ('Agreement', 'NumberAgreement', 'Zgodność liczby'),
                ('Agreement', 'PersonAgreement', 'Zgodność osoby'),

                -- Other grammatical meanings
                ('Other', 'Question', 'Funkcja pytająca'),
                ('Other', 'Exclamation', 'Funkcja wykrzyknikowa'),
                ('Other', 'VocativeAddress', 'Funkcja zwracania się do adresata'),
                ('Other', 'Conjunction', 'Funkcja łącząca'),
                ('Other', 'Coordination', 'Łączenie równorzędnych elementów'),
                ('Other', 'Subordination', 'Podporządkowanie składniowe'),
                ('Other', 'Conditionality', 'Warunkowość'),
                ('Other', 'Concession', 'Przyzwolenie'),
                ('Other', 'Purpose', 'Cel'),
                ('Other', 'Cause', 'Przyczyna'),
                ('Other', 'Result', 'Skutek'),
                ('Other', 'Comparison', 'Porównanie'),
                ('Other', 'Similarity', 'Podobieństwo'),
                ('Other', 'Difference', 'Różnica'),
                ('Other', 'Location', 'Lokalizacja'),
                ('Other', 'Direction', 'Kierunek'),
                ('Other', 'Origin', 'Pochodzenie'),
                ('Other', 'Destination', 'Cel ruchu'),
                ('Other', 'Possession', 'Posiadanie'),
                ('Other', 'Association', 'Powiązanie'),
                ('Other', 'Instrument', 'Narzędzie lub środek'),
                ('Other', 'Accompaniment', 'Towarzyszenie'),
                ('Other', 'Time', 'Określenie czasu'),
                ('Other', 'Duration', 'Czas trwania'),
                ('Other', 'Frequency', 'Częstotliwość'),
                ('Other', 'Manner', 'Sposób wykonania czynności'),
                ('Other', 'Measure', 'Miara lub stopień'),
                ('Other', 'Approximation', 'Przybliżenie'),
                ('Other', 'Intensification', 'Wzmocnienie intensywności'),
                ('Other', 'Attenuation', 'Osłabienie intensywności'),
                ('Other', 'Inchoative', 'Rozpoczęcie stanu lub czynności'),
                ('Other', 'Terminative', 'Zakończenie stanu lub czynności'),
                ('Other', 'Frequentative', 'Częstotliwość lub powtarzalność czynności'),
                ('Other', 'Reciprocal', 'Wzajemność czynności'),
                ('Other', 'ReflexiveMeaning', 'Czynność skierowana na wykonawcę'),
                ('Other', 'BenefactiveMeaning', 'Działanie na czyjąś korzyść'),
                ('Other', 'Malefactive', 'Działanie na czyjąś niekorzyść'),
                ('Other', 'Applicative', 'Włączenie dodatkowego argumentu do czynności'),
                ('Other', 'CausativeMeaning', 'Spowodowanie czynności lub stanu'),
                ('Other', 'Antipassive', 'Konstrukcja antypasywna'),
                ('Other', 'Converb', 'Forma łącząca czynności'),
                ('Other', 'Participle', 'Imiesłów'),
                ('Other', 'Gerund', 'Forma rzeczownikowa czasownika'),
                ('Other', 'Infinitive', 'Bezokolicznik'),
                ('Other', 'Supine', 'Supinum'),
                ('Other', 'VerbalNoun', 'Rzeczownik odczasownikowy'),
                ('Other', 'AdjectiveForm', 'Forma przymiotnikowa'),
                ('Other', 'AdverbialForm', 'Forma przysłówkowa');

            ";

            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();
        }


    }

}
