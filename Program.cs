/*
Author: Char(69) Dev Team
    - May
    - Sweden
    - Luke
Date:   
    02/13/2022
Demo:
    string inPt = "Hello Wolrd!";
    string encrypted = crypto.encrypt(inPt, crypto.secretKey);
    string decrypted = crypto.decrypt(encrypted, crypto.secretKey);
    Console.WriteLine(String.Format("Input : {0}\nEncrypted : {1}\nDecrypted : {2}\n", inPt, encrypted, decrypted));
*/
using c69_passwordManager.cryptography;
using System.IO;

namespace c69_passwordManager {

    public struct Password {
        public string username;
        public string password;
        public string url;
    }

    public class Program {
        private static bool isRunning = true;
        private static string defaultFilename = "./database/passwords.c69db";
        private static Cryptography crypto = new Cryptography();
        private static Dictionary<string, Password> passwords = new Dictionary<string, Password>();

        public static void Main(string[] args) {
            Console.WriteLine("Welcome to the C# Password Manager!");
            if(!File.Exists(defaultFilename))
                File.WriteAllText(defaultFilename, "");
            
            readFile(defaultFilename);
            UI();
            crypto.saveKey();
        }

        public static void killProc() {
            isRunning = false;
        }

        public static void writeFile(string file, List<string> content){
            try {
                StreamWriter sw = new StreamWriter(file);
                foreach (string line in content)
                    sw.WriteLine(line);
                sw.Close();
            }catch (IOException e){
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void saveFile(string file) {
            List<string> content = new List<string>();
            foreach (KeyValuePair<string, Password> entry in passwords) {
                content.Add(entry.Key + ":" + entry.Value.username + ":" + entry.Value.password);
            }
            writeFile(file, content);
        } 

        public static void readFile(string file){
            // read file lines
            try {
                StreamReader sr = new StreamReader(file);
                string line;
                while ((line = sr.ReadLine()) != null) {
                    string[] split = line.Split(':');
                    Password p = new Password();
                    p.url = split[0];
                    p.username = split[1];
                    p.password = split[2].TrimEnd();
                    passwords.Add(p.url, p);
                }
                sr.Close();


            }catch (IOException e){
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void makePass(string site, string accName,string password){
            Password pass = new Password();
            pass.username = accName;
            pass.password = crypto.encrypt(password, crypto.secretKey);
            pass.url = site;
            passwords.Add(site, pass);
        }

        public static void getPass(string site){
            if (passwords.ContainsKey(site)){
                Console.WriteLine(site + site.PadRight(10) + ": " + passwords[site].username + site.PadRight(20) + crypto.decrypt(passwords[site].password, crypto.secretKey));
            } else {
                Console.WriteLine("No password found for " + site);
            }
        }

        public static void listPass(){
            foreach (KeyValuePair<string, Password> entry in passwords) {
                Console.WriteLine(entry.Key + entry.Key.PadRight(10) + ": " + entry.Value.username + entry.Key.PadRight(20) + crypto.decrypt(entry.Value.password, crypto.secretKey));
            }
        }

        public static void UI(){
            while(isRunning){
                Console.Write("> ");
                string? input = Console.ReadLine();
                if(input == null)
                    continue; 
                switch(input.ToLower().Trim()){
                    case "list":
                        //list passwords
                        listPass();
                        break;

                    case "exit":
                        killProc();
                        break;

                    case "makepass":
                        //make pass
                        Console.Write("Site: ");
                        string? site = Console.ReadLine();
                        Console.Write("Account Name: ");
                        string? accName = Console.ReadLine();
                        Console.Write("Password: ");
                        string? password = Console.ReadLine();
                        
                        if (site == null || accName == null || password == null)
                            continue;

                        makePass(site, accName, password);
                        break;
                        
                    case "help":
                        Console.WriteLine("Commands:\nlist\tLists all passwords"+
                                        "\nSave\tSaves all passwords"+
                                        "\nexit\tExists the program"+
                                        "\nfindpass <site>\tFinds a password with the given site"+
                                        "\nmakepass <site> <accName> <pass>\tMakes a new password with the given site, account name and password"+
                                        "\ndeletepass <site>\tDeletes a password"+
                                        "\nhelp\tShows this help menu");
                        break;
                        
                    case "deletepass":
                        //delete pass
                        Console.Write("Site: ");
                        string? site2 = Console.ReadLine();
                        if (site2 == null)
                            continue;
                        if (passwords.ContainsKey(site2)){
                            passwords.Remove(site2);
                            Console.WriteLine("Password deleted");
                        } else {
                            Console.WriteLine("No password found for " + site2);
                        }
                        break;

                    case "findpass":
                        //find pass
                        Console.Write("Site: ");
                        string? site3 = Console.ReadLine();
                        if (site3 == null)
                            continue;
                        getPass(site3);
                        break;

                    case "save":
                        //save
                        saveFile(defaultFilename);
                        break;

                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }
            }
            
        } 
    }
}
