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

# Don't be stupid
You are responsible of what you will do with this, do not trust me read the code and be sure you understand what you are about to run.
