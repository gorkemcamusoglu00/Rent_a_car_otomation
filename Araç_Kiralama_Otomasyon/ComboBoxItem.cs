namespace Araç_Kiralama_Otomasyon
{
    internal class ComboBoxItem
    {
        public int Car_id { get; set; }
        public string DisplayText { get; set; }

        public override string ToString()
        {
            return DisplayText; // ComboBox'ta bu metin gösterilecek
        }
    }
}