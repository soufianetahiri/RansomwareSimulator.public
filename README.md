# RansomwareSimulator
The goal of this repository is to provide a simple, harmless way to check your AV's protection on ransomware.

This tool simulates typical ransomware behaviour, such as:

-  Exfiltrating Documents (SMTP and/or FTP)
-   Creating/Deleting Volume Shadow Copies
-   Encrypting documents (I removed AES support just in case)
-   Dropping a ransomware note to the user's desktop

You can  configure your simulation via `appsettings.json`

    {
      "Config": {
        "ExtensionsToEncrypt": [ ".doc", ".docx", ".ppt", ".pptx", ".xls", ".xlsx", ".txt" ],
        "WorkingDirectory": "C:\\Users\\\\demo",
        "MaxFileSearch": 1,
        "FTPUser": "xx@xx.xx",
        "FTPPassword": "xx",
        "FTPHost": "ftp://ftp.xx.xx",
        "SMTPServer": "smtp.gmail.com",
        "SMTPUser": "xx@gmail.com",
        "SMTPPwd": "xxx",
        "SMTPSender": "xx@gmail.com",
        "SMTPReciever": "xx@gmail.com",
        "SMTPPort": 587,
        "CreateDeleteShadow": true
      }
    }

 - `ExtensionsToEncrypt`: The file extensions the ransomware simulator will search and encrypt
 - `WorkingDirectory`: Where to search those files
 - `MaxFileSearch`: Number of files to encrypt

The ransomware simulator takes no action that actually encrypts pre-existing files on the device, or deletes Volume Shadow Copies. However, any AV products looking for such behaviour should still hopefully trigger.

Each step, as listed above, can also be either disabled or "highly" configured. I recommand you have a look at the code before running it.

Every file is backed-up before encryption (just in case), it uses a simple `XOR` with the key `000`. Every taken  step is logged into **RansomSimulator.log**

# Execution log example

        2022-06-16 15:28:54,438
     [START] List Backup and Encrypt files with the following extensions .doc ;.docx ;.ppt ;.pptx ;.xls ;.xlsx ;.txt On C:\Users\soufiane\demo. Files count 12
    
    2022-06-16 15:28:54,452
     A backup of C:\Users\soufiane\demo\demo.docx was made here => C:\Users\soufiane\demo\\demo.docx.backup
     A backup of C:\Users\soufiane\demo\demo.pptx was made here => C:\Users\soufiane\demo\\demo.pptx.backup
     A backup of C:\Users\soufiane\demo\demo.txt was made here => C:\Users\soufiane\demo\\demo.txt.backup
     A backup of C:\Users\soufiane\demo\demo_ransom (1).docx was made here => C:\Users\soufiane\demo\\demo_ransom (1).docx.backup
     A backup of C:\Users\soufiane\demo\demo_ransom (1).pptx was made here => C:\Users\soufiane\demo\\demo_ransom (1).pptx.backup
     A backup of C:\Users\soufiane\demo\demo_ransom (1).txt was made here => C:\Users\soufiane\demo\\demo_ransom (1).txt.backup
     A backup of C:\Users\soufiane\demo\demo_ransom (2).docx was made here => C:\Users\soufiane\demo\\demo_ransom (2).docx.backup
     A backup of C:\Users\soufiane\demo\demo_ransom (2).pptx was made here => C:\Users\soufiane\demo\\demo_ransom (2).pptx.backup
     A backup of C:\Users\soufiane\demo\demo_ransom (2).txt was made here => C:\Users\soufiane\demo\\demo_ransom (2).txt.backup
     A backup of C:\Users\soufiane\demo\invoice.docx was made here => C:\Users\soufiane\demo\\invoice.docx.backup
     A backup of C:\Users\soufiane\demo\prez.pptx was made here => C:\Users\soufiane\demo\\prez.pptx.backup
    
    2022-06-16 15:28:54,484
     A backup of C:\Users\soufiane\demo\pwds.txt was made here => C:\Users\soufiane\demo\\pwds.txt.backup
     C:\Users\soufiane\demo\demo.docx successfully copied to C:\Users\soufianeRansomwareSimulator_2022-06-16_15-28-54-512\demo.docx
     C:\Users\soufiane\demo\demo.pptx successfully copied to C:\Users\soufianeRansomwareSimulator_2022-06-16_15-28-54-512\demo.pptx
     C:\Users\soufiane\demo\demo.txt successfully copied to C:\Users\soufianeRansomwareSimulator_2022-06-16_15-28-54-512\demo.txt
     C:\Users\soufiane\demo\demo_ransom (1).docx successfully copied to C:\Users\soufianeRansomwareSimulator_2022-06-16_15-28-54-512\demo_ransom (1).docx
     C:\Users\soufiane\demo\demo_ransom (1).pptx successfully copied to C:\Users\soufianeRansomwareSimulator_2022-06-16_15-28-54-512\demo_ransom (1).pptx
     C:\Users\soufiane\demo\demo_ransom (1).txt successfully copied to C:\Users\soufianeRansomwareSimulator_2022-06-16_15-28-54-512\demo_ransom (1).txt
     C:\Users\soufiane\demo\demo_ransom (2).docx successfully copied to C:\Users\soufianeRansomwareSimulator_2022-06-16_15-28-54-512\demo_ransom (2).docx
     C:\Users\soufiane\demo\demo_ransom (2).pptx successfully copied to C:\Users\soufianeRansomwareSimulator_2022-06-16_15-28-54-512\demo_ransom (2).pptx
     C:\Users\soufiane\demo\demo_ransom (2).txt successfully copied to C:\Users\soufianeRansomwareSimulator_2022-06-16_15-28-54-512\demo_ransom (2).txt
    
    2022-06-16 15:28:54,556
     C:\Users\soufiane\demo\invoice.docx successfully copied to C:\Users\soufianeRansomwareSimulator_2022-06-16_15-28-54-512\invoice.docx
     C:\Users\soufiane\demo\prez.pptx successfully copied to C:\Users\soufianeRansomwareSimulator_2022-06-16_15-28-54-512\prez.pptx
     C:\Users\soufiane\demo\pwds.txt successfully copied to C:\Users\soufianeRansomwareSimulator_2022-06-16_15-28-54-512\pwds.txt
     Archive created : C:\Users\soufianeRansomwareSimulator_2022-06-16_15-28-54-512.zip
     The file C:\Users\soufianeRansomwareSimulator_2022-06-16_15-28-54-512.zip was sent to ftp://ftp.xxxxe.wxxxx
     
    2022-06-16 15:28:58,141
     Something went wrong: The SMTP server requires a secure connection or the client was not authenticated. The server response was: 5.7.0 Authentication Required. Learn more at
    
    2022-06-16 15:28:58,142
     Error while deleting C:\Users\soufianeRansomwareSimulator_2022-06-16_15-28-54-512.zip. The process cannot access the file 'C:\Users\soufianeRansomwareSimulator_2022-06-16_15-28-54-512.zip' because it is being used by another process.
    
    2022-06-16 15:28:58,152
     Starting Encryption using the Password 000!
     
    2022-06-16 15:28:58,169
     Encrypting C:\Users\soufiane\demo\demo.docx as C:\Users\soufiane\demo\demo.docx.xor
    2022-06-16 15:28:58,169
     Encrypting C:\Users\soufiane\demo\demo_ransom (2).docx as C:\Users\soufiane\demo\demo_ransom (2).docx.xor
    2022-06-16 15:28:58,169
     Encrypting C:\Users\soufiane\demo\demo_ransom (2).pptx as C:\Users\soufiane\demo\demo_ransom (2).pptx.xor
    2022-06-16 15:28:58,169
     Encrypting C:\Users\soufiane\demo\demo_ransom (1).pptx as C:\Users\soufiane\demo\demo_ransom (1).pptx.xor
    2022-06-16 15:28:58,169
     Encrypting C:\Users\soufiane\demo\demo_ransom (1).docx as C:\Users\soufiane\demo\demo_ransom (1).docx.xor
    2022-06-16 15:28:58,169
     Encrypting C:\Users\soufiane\demo\demo_ransom (2).txt as C:\Users\soufiane\demo\demo_ransom (2).txt.xor
    2022-06-16 15:28:58,169
     Encrypting C:\Users\soufiane\demo\demo.txt as C:\Users\soufiane\demo\demo.txt.xor
    2022-06-16 15:28:58,169
     Encrypting C:\Users\soufiane\demo\demo.pptx as C:\Users\soufiane\demo\demo.pptx.xor
    2022-06-16 15:28:58,169
     Encrypting C:\Users\soufiane\demo\demo_ransom (1).txt as C:\Users\soufiane\demo\demo_ransom (1).txt.xor
    2022-06-16 15:28:58,187
     Encrypting C:\Users\soufiane\demo\invoice.docx as C:\Users\soufiane\demo\invoice.docx.xor
    2022-06-16 15:28:58,189
     Encrypting C:\Users\soufiane\demo\prez.pptx as C:\Users\soufiane\demo\prez.pptx.xor
    2022-06-16 15:28:58,192
     Encrypting C:\Users\soufiane\demo\pwds.txt as C:\Users\soufiane\demo\pwds.txt.xor
    
    2022-06-16 15:28:58,217
    Summary of Encryption:
    
     C:\Users\soufiane\demo\demo.docx
    	|_C:\Users\soufiane\demo\demo.docx.xor
     C:\Users\soufiane\demo\demo_ransom (2).docx
    	|_C:\Users\soufiane\demo\demo_ransom (2).docx.xor
     C:\Users\soufiane\demo\demo_ransom (2).pptx
    	|_C:\Users\soufiane\demo\demo_ransom (2).pptx.xor
     C:\Users\soufiane\demo\demo_ransom (1).pptx
    	|_C:\Users\soufiane\demo\demo_ransom (1).pptx.xor
     C:\Users\soufiane\demo\demo_ransom (1).docx
    	|_C:\Users\soufiane\demo\demo_ransom (1).docx.xor
     C:\Users\soufiane\demo\demo_ransom (2).txt
    	|_C:\Users\soufiane\demo\demo_ransom (2).txt.xor
     C:\Users\soufiane\demo\demo.txt
    	|_C:\Users\soufiane\demo\demo.txt.xor
     C:\Users\soufiane\demo\demo.pptx
    	|_C:\Users\soufiane\demo\demo.pptx.xor
     C:\Users\soufiane\demo\invoice.docx
    	|_C:\Users\soufiane\demo\invoice.docx.xor
     C:\Users\soufiane\demo\demo_ransom (1).txt
    	|_C:\Users\soufiane\demo\demo_ransom (1).txt.xor
     C:\Users\soufiane\demo\prez.pptx
    	|_C:\Users\soufiane\demo\prez.pptx.xor
     C:\Users\soufiane\demo\pwds.txt
    	|_C:\Users\soufiane\demo\pwds.txt.xor
    
    2022-06-16 15:28:58,318
     
    Shadow Copies found:
    \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy12 =>{88A47CA7-BBDE-4EC0-AAB4-4399834F46F8}
    \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy13 =>{70FDC574-2E7F-4616-B639-43FD1EEF55D4}
    \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy14 =>{DAF51CB7-EA8B-4B9F-AE93-A1677A4CC3EC}
    \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy15 =>{BC60AA8C-107E-4968-A938-3926E084975B}
    \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy16 =>{5917EC80-B43E-4251-A9E4-57D5E0225ED2}
    \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy17 =>{4DB71A97-2B2B-4E99-A53E-1F36225A65FF}
    \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy18 =>{6FE27CD5-125F-409C-B0EE-7645712D2F9A}
    \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy19 =>{AB121E85-98C4-4614-9B2F-68745BB78AD5}
    \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy39 =>{21570201-DE73-4FB9-958D-391521A35B33}
    \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy43 =>{8E08A366-D66F-4382-860F-814D17EE3E23}
    
    Shadow copy created:: {95A07A67-5028-4E63-9641-0A1AFA808893}
    2022-06-16 15:29:07,989
     {95A07A67-5028-4E63-9641-0A1AFA808893} successfully deleted

# Don't be stupid
You are responsible of what you will do with this, do not trust me read the code and be sure you understand what you are about to run.
