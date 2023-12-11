namespace SdtdServerKit
{
    public static class Utils
    {
        public static int DaysRemaining(int daysUntilHorde)
        {
            int bloodmoonFrequency = GamePrefs.GetInt(EnumGamePrefs.BloodMoonFrequency);
            if (daysUntilHorde <= bloodmoonFrequency)
            {
                int daysLeft = bloodmoonFrequency - daysUntilHorde;
                return daysLeft;
            }
            else
            {
                int daysLeft = daysUntilHorde - bloodmoonFrequency;
                return DaysRemaining(daysLeft);
            }
        }
    }
}