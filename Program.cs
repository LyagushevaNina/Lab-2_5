#pragma warning disable IDE0028
#pragma warning disable IDE0090
internal class Program
{
    [STAThread]
    private static void Main()
    {
        //5-22 1+x2+x3+x6
        bool[] key = [true, true, true, false, false, true];

        while (true)
        {
            Console.WriteLine("0 - Вывод всех коллизий");
            Console.WriteLine("1 - Выход");
            int choice = SafeReadInteger();
            Console.WriteLine("Введите байт:");
            byte encByte = (byte)SafeReadInteger();

            switch (choice)
            {
                case 0:
                    bool[] encBit = CRC(encByte, key);
                    bool[][] mass = new bool[256][];

                    for (int i = 0; i < 256; i++)
                    {
                        mass[i] = CRC((byte)i, key);
                    }

                    Dictionary<string, List<int>> hashTable = new Dictionary<string, List<int>>();

                    for (int i = 0; i < 256; i++)
                    {
                        string hash = string.Join("", mass[i].Select(b => b ? 1 : 0));

                        if (!hashTable.TryGetValue(hash, out List<int>? value))
                        {
                            value = new List<int>();
                            hashTable[hash] = value;
                        }

                        value.Add(i);
                    }
                    // Числа, у которых одинаковый хеш
                    foreach (KeyValuePair<string, List<int>> entry in hashTable)
                    {
                        if (entry.Value.Count > 1)
                        {
                            Console.WriteLine($"Хеш: {entry.Key}, Числа: ");
                            Console.WriteLine(string.Join(", ", entry.Value));
                            Console.WriteLine();
                        }
                    }

                    Console.WriteLine($"CRC для байта {encByte} и маски {(string.Join("", key.Select(b => b ? 1 : 0)))} равен {(string.Join("", CRC(encByte, key).Select(b => b ? 1 : 0)))}");

                    break;

                case 1:
                    return;

                default:
                    Console.WriteLine("ОШИБКА: Неверный номер.");
                    break;
            }
        }
    }

    private static bool[] ConvertByteToBoolArray(int b) // Преобразует байт в массив 8 бит
    {
        bool[] result = new bool[8];
        for (int i = 0; i < 8; i++)
        {
            result[i] = (b & (1 << i)) != 0;
        }

        Array.Reverse(result);
        return result;
    }

    private static int SafeReadInteger()
    {
        while (true)
        {
#pragma warning disable CS8600
            string sValue = Console.ReadLine();
            if (int.TryParse(sValue, out int iValue))
            {
                return iValue;
            }

            Console.WriteLine("ОШИБКА: Неверный формат. Введите целочисленное значение...");
        }
    }

    private static bool XOR(bool a, bool b)
    {
        return a != b;
    }

    internal static bool[] CRC(byte a, bool[] key)
    {
        bool[] b = ConvertByteToBoolArray(a);
        Array.Resize(ref b, 11); // Добавление 3 нулей в конце

        for (int i = 0; i < 8; i++)
        {
            if (b[i])
            {
                for (int j = 0; j < 6; j++)
                {
                    if (i + j < 11) // Проверка на выход за границы массива b
                    {
                        b[i + j] = XOR(b[i + j], key[j]);
                    }
                }
            }
        }

        bool[] c = [b[8], b[9], b[10]];
        return c;
    }
}
