namespace Databasprojekt;


// Detta kommer vara en demo-klass som visar symmetrisk kryptering
// klartext -> krypterar -> lagra -> läsa -> dekryptera - klartext
public class EncryptionHelper
{
    
    // En grundläggande
    // 0x42 är hexadecimalt (bas 16). Det motsvarar 66 bytes i decimal (bas 10)
    // Värdet är taget från påhittat
    private const byte Key = 0x42; // 66 bytes

    public static string Encrypt(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return text;
        }
        
        // 1 Konvertera texten till bytes
        // Varför? Texten är Unicode (char/strings)
        // XOR för att kunna förvränga vår sträng och då behöver vi omvandla texten till en byte array
        var bytes = System.Text.Encoding.UTF8.GetBytes(text);
        
        // 2. 
        // Varför just XOR? 
        // - Enkelt att förstå 
        // - Symmetriskt: (A ^ K) ^ K = A
        
        // Varför (byte)(bytes[i]) ^ Key)
        // - bytes[i] är en byte (0-255)
        // - Key är också en byte
        // - bytes[i] ^ Key ger ett int-resultat, så vi castar tillbaka det till byte
        
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = (byte)(bytes[i] ^ Key);
        }
        // 3 För att kunna sparar resultatet som text. Kodar vi bytes till base64
        // Varför Base64?
        // Efter vi har gjort XOR kan bytes innehålla obegripliga eller ogiltiga tecken för text/JSON.
        // Lättare att lagra i filer med JSON, databser osv.
        return Convert.ToBase64String(bytes);
    }

    public static string Decrypt(string krypteradtext)
    {
        // 1
        if (string.IsNullOrEmpty(krypteradtext))
        {
            return krypteradtext;
        }
        
        //2
        //Gör om base64 strängen till bytes igen
        // XOR tilbaka med samma nyckel
        // Här utnyttjar vi XOR-Egenskapen
        // originaltext ^ Key = Krypterad
        // Krypteradtext ^ Key = original
        // Därför ser koden exakt likadan ut
        
        var bytes = Convert.FromBase64String(krypteradtext);

        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = (byte)(bytes[i] ^ Key);
        }
        
        // 3 Konverterar tillbaka frpn bytes -> klartext med UTF8
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

}