using System;
using System.IO;
using CodeFluent.Runtime.BinaryServices;
using Microsoft.Win32;


namespace malware
{
    class Program
    {
        public static string INSTRUCT = "InStrucT1onss.txt";
        public static string originalPath = @"C:\Windows\Temp\OriginalPath.txt";
        public static string EncPath = @"C:\Windows\Temp\EncPath.txt";
        public static string DeletedItems = @"C:\Windows\Temp\DeletedItems.txt";
        public static string endFile;
        public static string[] strs = File.ReadAllLines(originalPath);
        public static string[] dels = File.ReadAllLines(DeletedItems);
        public static string baseName = "";

        static void Main(string[] args)
        {




            foreach (var p in dels)
            {
                baseName = Path.GetFileName(p);
                string fullPath = "";
                if (File.Exists(Path.GetDirectoryName(p) + "\\" + INSTRUCT))
                {
                    fullPath = Path.GetDirectoryName(p) + "\\" + INSTRUCT + ":" + baseName;
                    string stream = NtfsAlternateStream.ReadAllText(fullPath);

                    if (File.Exists(p))
                    {

                        string content = File.ReadAllText(p);
                        

                    }
                    else
                    {
                        using (StreamWriter streamWriter = new StreamWriter(p))
                        {
                            streamWriter.WriteLine(stream);
                            
                        }
                    }



                }
            }




            foreach (var i in strs)
            {
                
                endFile = ChangeExt(i, ".gg");
                

                if (File.Exists(endFile))

                {

                    var str = File.ReadAllText(endFile).Trim();
                    

                    byte[] byt = Ascii85.Decode(str);
                    if (File.Exists(i))
                    {
                        File.Delete(i);
                    }


                    FileStream fs = new FileStream(i, FileMode.Create);

                    fs.Write(byt, 0, byt.Length);

                    File.Delete(endFile);
                    Console.WriteLine("{0} has been decoded", i);

                    fs.Close();
                }
                else
                {
                    if (File.Exists(i))
                    {
                        Console.WriteLine("original file {0} is exist", i);
                    }
                    else
                    {
                        Console.WriteLine("original file {0} can not been found", i);
                    }
                }
            }
            string[] drives = Directory.GetLogicalDrives();

            foreach (string s in drives)
            {
                DeleteFile(s);
            }
            D_File(originalPath);
            D_File(EncPath);
            D_File(DeletedItems);

            Console.WriteLine("Delete Files Complete!");
            DeletelmKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "RunGG");
            DeletecuKey(@"SOFTWARE\Microsoft\win", "encNum");
            DeletecuKey(@"SOFTWARE\Microsoft\Services", "IVvalue");
            DeletecuKey(@"SOFTWARE\Microsoft\Services", "KeyValue");
            Console.WriteLine("complete, press enter to quit");
            Console.ReadLine();
        }
        public static string ChangeExt(string oriFile, string ext)
        {
            string previousPath = Path.GetDirectoryName(oriFile);
            string previousFile = Path.GetFileNameWithoutExtension(oriFile);
            endFile = previousPath + "\\" + previousFile + ext;
            return endFile;
        }



        public static void DeletecuKey(String path, String value)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true))
            {
                if (key != null)
                {
                    try
                    {
                        key.DeleteValue(value);
                        Console.WriteLine("The registory key {0} has been deleted", value);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                    }
                }
            }

        }
        public static void DeletelmKey(String path, String value)
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(path, true))
            {
                if (key != null)
                {
                    try
                    {
                        key.DeleteValue(value);
                        Console.WriteLine("The registory key {0} has been deleted", value);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                    }
                }
            }

        }
        public static void D_File(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                Console.WriteLine("The file {0} has been deleted", path);
            }
        }

        public static void DeleteFile(string dirRoot)
        {
            string deleteFileName = "InStrucT1onss.txt";
            try
            {
                string[] rootDirs = Directory.GetDirectories(dirRoot);
                string[] rootFiles = Directory.GetFiles(dirRoot);

                foreach (string s2 in rootFiles)
                {
                    if (s2.Contains(deleteFileName))
                    {

                        File.Delete(s2);
                        Console.WriteLine("{0} has been deleted", s2);
                    }
                }
                foreach (string s1 in rootDirs)
                {
                    DeleteFile(s1);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

        }

    }


    public static class Ascii85
    {

        public static byte[] Decode(string encoded)
        {
            if (encoded == null)
                throw new ArgumentNullException("encoded");


            using (MemoryStream stream = new MemoryStream(encoded.Length * 4 / 5))
            {

                int count = 0;
                uint value = 0;
                foreach (char ch in encoded)
                {
                    if (ch == 'z' && count == 0)
                    {

                        DecodeValue(stream, value, 0);
                    }
                    else if (ch < c_firstCharacter || ch > c_lastCharacter)
                    {
                        throw new FormatException($"Invalid character '{ch}' in Ascii85 block.");
                    }
                    else
                    {

                        try
                        {
                            checked { value += (uint)(s_powersOf85[count] * (ch - c_firstCharacter)); }
                        }
                        catch (OverflowException ex)
                        {
                            throw new FormatException("The current group of characters decodes to a value greater than UInt32.MaxValue.", ex);
                        }

                        count++;


                        if (count == 5)
                        {
                            DecodeValue(stream, value, 0);
                            count = 0;
                            value = 0;
                        }
                    }
                }

                if (count == 1)
                {
                    throw new FormatException("The final Ascii85 block must contain more than one character.");
                }
                else if (count > 1)
                {

                    for (int padding = count; padding < 5; padding++)
                    {
                        try
                        {
                            checked { value += 84 * s_powersOf85[padding]; }
                        }
                        catch (OverflowException ex)
                        {
                            throw new FormatException("The current group of characters decodes to a value greater than UInt32.MaxValue.", ex);
                        }
                    }
                    DecodeValue(stream, value, 5 - count);
                }

                return stream.ToArray();
            }
        }

        private static void DecodeValue(Stream stream, uint value, int paddingChars)
        {
            stream.WriteByte((byte)(value >> 24));
            if (paddingChars == 3)
                return;
            stream.WriteByte((byte)((value >> 16) & 0xFF));
            if (paddingChars == 2)
                return;
            stream.WriteByte(((byte)((value >> 8) & 0xFF)));
            if (paddingChars == 1)
                return;
            stream.WriteByte((byte)(value & 0xFF));
        }


        const char c_firstCharacter = '!';
        const char c_lastCharacter = 'u';

        static readonly uint[] s_powersOf85 = new uint[] { 85u * 85u * 85u * 85u, 85u * 85u * 85u, 85u * 85u, 85u, 1 };
    }
}

