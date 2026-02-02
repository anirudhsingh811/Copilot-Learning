# ?? Welcome to AZ-204 Azure Security Module!

## ?? Get Started in 3 Steps

### 1?? Run the Application
```powershell
dotnet run
```

### 2?? Try Option [4] - Managed Identity
This demo works immediately with no setup required!

### 3?? Follow the Setup Guide
After exploring, check **[7] Setup Guide** in the menu to create Azure resources.

---

## ?? Documentation Files

Choose based on your learning style:

| File | Purpose | Time | When to Use |
|------|---------|------|-------------|
| **QUICKSTART.md** | Fast setup | 10 min | Want to get running ASAP |
| **README.md** | Complete guide | 30 min | Want full understanding |
| **SETUP_CHECKLIST.md** | Track progress | 30 min | Like organized checklists |
| **PROJECT_SUMMARY.md** | Overview | 5 min | Want to see what's included |

---

## ?? What's Included

This project contains **real, runnable examples** for:

? **Azure Key Vault** - Secrets, Keys, Certificates  
? **MSAL Authentication** - OAuth 2.0 flows  
? **Microsoft Graph API** - User data, emails, calendar  
? **Managed Identities** - Secure authentication  
? **Shared Access Signatures** - Storage security  

**Exam Weight:** This covers 20-25% of AZ-204 certification!

---

## ? Quick Commands

```powershell
# Run the application
dotnet run

# Setup user secrets (recommended for configuration)
dotnet user-secrets init
dotnet user-secrets set "Azure:TenantId" "your-value"
dotnet user-secrets set "Azure:ClientId" "your-value"

# Login to Azure
az login

# View your configuration
dotnet user-secrets list
```

---

## ?? Fast Start Paths

### Path 1: "Just Show Me" (5 minutes)
1. `dotnet run`
2. Press **[4]** for Managed Identity demo
3. Read the educational content
4. Press **[8]** for Exam Tips

### Path 2: "Let's Build Something" (1 hour)
1. Read **QUICKSTART.md**
2. Run setup commands for Azure resources
3. Configure the app
4. Run all demos **[6]**

### Path 3: "I Want to Master This" (3 hours)
1. Read **README.md** fully
2. Use **SETUP_CHECKLIST.md** to track progress
3. Run each demo individually
4. Explore the code in `Services/` folder
5. Review **[8] Exam Tips** in the app

---

## ?? What to Expect

When you run `dotnet run`, you'll see:

```
?????????????????????????????????????????????????????????????????????
?         AZ-204: Implement Azure Security                         ?
?         Interactive Learning & Hands-On Demos                    ?
?????????????????????????????????????????????????????????????????????

??  Exam Weight: 20-25% (HIGHEST PRIORITY!)

?? Choose a demo to run:

  [1] ?? Azure Key Vault
  [2] ?? MSAL Authentication
  [3] ?? Microsoft Graph API
  [4] ?? Managed Identity
  [5] ?? Shared Access Signatures (SAS)
  [6] ?? Run All Demos
  [7] ?? Setup Guide
  [8] ?? AZ-204 Exam Tips
  [Q] ? Exit
```

---

## ?? Prerequisites

**Minimum to Run:**
- ? .NET 9 SDK (already have this!)
- ? That's it! Some demos work immediately.

**For Full Experience:**
- Azure subscription (free tier works)
- Azure CLI installed
- 30 minutes to setup Azure resources

---

## ?? Need Help?

**In the application:**
- Press **[7]** for complete setup instructions
- Press **[8]** for exam tips and key concepts

**In documentation:**
- **QUICKSTART.md** - Fast setup guide
- **README.md** - Troubleshooting section
- **SETUP_CHECKLIST.md** - Step-by-step checklist

**Common issues:**
- "Key Vault access denied" ? Run setup commands in QUICKSTART.md
- "MSAL error" ? Check [7] Setup Guide for App Registration
- "Storage error" ? Set environment variables for storage account

---

## ?? Learning Goals

By completing this module, you'll be able to:
- ? Manage secrets securely with Key Vault
- ? Implement OAuth 2.0 authentication with MSAL
- ? Call Microsoft Graph APIs
- ? Use Managed Identities for Azure resources
- ? Generate and validate SAS tokens
- ? Apply Azure security best practices
- ? Pass AZ-204 security questions confidently!

---

## ?? Recommended First Steps

1. **Right Now:** Run `dotnet run` and press **[4]**
2. **Next 10 min:** Read **QUICKSTART.md**
3. **Next 20 min:** Create Azure resources
4. **Next 30 min:** Run all demos **[6]**
5. **Then:** Explore code and prepare for certification!

---

## ?? Project Stats

- **5** Complete service implementations
- **15+** Demo scenarios
- **2,000+** Lines of production-ready code
- **3** Comprehensive guides
- **100%** AZ-204 security topic coverage

---

## ?? Why This Project is Special

? **Real Azure Operations** - Not mocked or simulated  
? **Production Code** - Best practices demonstrated  
? **Exam-Focused** - Aligned with AZ-204 requirements  
? **Self-Contained** - Everything in one place  
? **Well-Documented** - Multiple learning resources  
? **Error-Friendly** - Helpful messages guide you  

---

## ?? Ready to Start?

```powershell
# Let's go!
dotnet run
```

**Choose option [4] or [7] to begin your Azure Security journey! ??**

---

*?? Tip: Star this project and bookmark the documentation files for easy reference!*

**Good luck with your AZ-204 certification! ??**
