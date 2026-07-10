using System.Collections.Generic;

namespace RecycleBinTaskbar.Shared
{
    /// <summary>
    /// All the display strings needed for a single language: the shortcut's
    /// description tooltip plus the title/description of each Jump List entry.
    /// </summary>
    public class LanguageStrings
    {
        public string Code { get; set; }
        public string NativeName { get; set; }
        public string ShortcutDescription { get; set; }
        public string EmptyTitle { get; set; }
        public string EmptyDescription { get; set; }
        public string OpenTitle { get; set; }
        public string OpenDescription { get; set; }
    }

    public static class Localization
    {
        public const string DefaultLanguageCode = "en";

        public static readonly Dictionary<string, LanguageStrings> Languages = new()
        {
            ["en"] = new LanguageStrings
            {
                Code = "en", NativeName = "English",
                ShortcutDescription = "Recycle Bin",
                EmptyTitle = "Empty Recycle Bin", EmptyDescription = "Empty the Recycle Bin",
                OpenTitle = "Open Recycle Bin", OpenDescription = "Open the Recycle Bin"
            },
            ["es"] = new LanguageStrings
            {
                Code = "es", NativeName = "Español",
                ShortcutDescription = "Papelera de reciclaje",
                EmptyTitle = "Vaciar papelera de reciclaje", EmptyDescription = "Vaciar la papelera de reciclaje",
                OpenTitle = "Abrir papelera de reciclaje", OpenDescription = "Abrir la papelera de reciclaje"
            },
            ["fr"] = new LanguageStrings
            {
                Code = "fr", NativeName = "Français",
                ShortcutDescription = "Corbeille",
                EmptyTitle = "Vider la corbeille", EmptyDescription = "Vider la corbeille",
                OpenTitle = "Ouvrir la corbeille", OpenDescription = "Ouvrir la corbeille"
            },
            ["de"] = new LanguageStrings
            {
                Code = "de", NativeName = "Deutsch",
                ShortcutDescription = "Papierkorb",
                EmptyTitle = "Papierkorb leeren", EmptyDescription = "Den Papierkorb leeren",
                OpenTitle = "Papierkorb öffnen", OpenDescription = "Den Papierkorb öffnen"
            },
            ["it"] = new LanguageStrings
            {
                Code = "it", NativeName = "Italiano",
                ShortcutDescription = "Cestino",
                EmptyTitle = "Svuota cestino", EmptyDescription = "Svuota il cestino",
                OpenTitle = "Apri cestino", OpenDescription = "Apri il cestino"
            },
            ["pt"] = new LanguageStrings
            {
                Code = "pt", NativeName = "Português",
                ShortcutDescription = "Lixeira",
                EmptyTitle = "Esvaziar lixeira", EmptyDescription = "Esvaziar a lixeira",
                OpenTitle = "Abrir lixeira", OpenDescription = "Abrir a lixeira"
            },
            ["ru"] = new LanguageStrings
            {
                Code = "ru", NativeName = "Русский",
                ShortcutDescription = "Корзина",
                EmptyTitle = "Очистить корзину", EmptyDescription = "Очистить корзину",
                OpenTitle = "Открыть корзину", OpenDescription = "Открыть корзину"
            },
            ["zh-Hans"] = new LanguageStrings
            {
                Code = "zh-Hans", NativeName = "简体中文",
                ShortcutDescription = "回收站",
                EmptyTitle = "清空回收站", EmptyDescription = "清空回收站",
                OpenTitle = "打开回收站", OpenDescription = "打开回收站"
            },
            ["ja"] = new LanguageStrings
            {
                Code = "ja", NativeName = "日本語",
                ShortcutDescription = "ごみ箱",
                EmptyTitle = "ごみ箱を空にする", EmptyDescription = "ごみ箱を空にします",
                OpenTitle = "ごみ箱を開く", OpenDescription = "ごみ箱を開きます"
            },
            ["ko"] = new LanguageStrings
            {
                Code = "ko", NativeName = "한국어",
                ShortcutDescription = "휴지통",
                EmptyTitle = "휴지통 비우기", EmptyDescription = "휴지통을 비웁니다",
                OpenTitle = "휴지통 열기", OpenDescription = "휴지통을 엽니다"
            },
            ["nl"] = new LanguageStrings
            {
                Code = "nl", NativeName = "Nederlands",
                ShortcutDescription = "Prullenbak",
                EmptyTitle = "Prullenbak legen", EmptyDescription = "De prullenbak legen",
                OpenTitle = "Prullenbak openen", OpenDescription = "De prullenbak openen"
            },
            ["pl"] = new LanguageStrings
            {
                Code = "pl", NativeName = "Polski",
                ShortcutDescription = "Kosz",
                EmptyTitle = "Opróżnij kosz", EmptyDescription = "Opróżnia kosz",
                OpenTitle = "Otwórz kosz", OpenDescription = "Otwiera kosz"
            },
            ["tr"] = new LanguageStrings
            {
                Code = "tr", NativeName = "Türkçe",
                ShortcutDescription = "Geri Dönüşüm Kutusu",
                EmptyTitle = "Geri Dönüşüm Kutusunu Boşalt", EmptyDescription = "Geri dönüşüm kutusunu boşaltır",
                OpenTitle = "Geri Dönüşüm Kutusunu Aç", OpenDescription = "Geri dönüşüm kutusunu açar"
            },
            ["ar"] = new LanguageStrings
            {
                Code = "ar", NativeName = "العربية",
                ShortcutDescription = "سلة المحذوفات",
                EmptyTitle = "إفراغ سلة المحذوفات", EmptyDescription = "إفراغ سلة المحذوفات",
                OpenTitle = "فتح سلة المحذوفات", OpenDescription = "فتح سلة المحذوفات"
            },
        };

        public static LanguageStrings Get(string code)
        {
            if (code != null && Languages.TryGetValue(code, out var strings))
                return strings;
            return Languages[DefaultLanguageCode];
        }
    }
}
