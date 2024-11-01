using System.Reflection;
using MelonLoader;

[assembly: AssemblyTitle(TheForest_TrueGear.BuildInfo.Description)]
[assembly: AssemblyDescription(TheForest_TrueGear.BuildInfo.Description)]
[assembly: AssemblyCompany(TheForest_TrueGear.BuildInfo.Company)]
[assembly: AssemblyProduct(TheForest_TrueGear.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + TheForest_TrueGear.BuildInfo.Author)]
[assembly: AssemblyTrademark(TheForest_TrueGear.BuildInfo.Company)]
[assembly: AssemblyVersion(TheForest_TrueGear.BuildInfo.Version)]
[assembly: AssemblyFileVersion(TheForest_TrueGear.BuildInfo.Version)]
[assembly: MelonInfo(typeof(TheForest_TrueGear.TheForest_TrueGear), TheForest_TrueGear.BuildInfo.Name, TheForest_TrueGear.BuildInfo.Version, TheForest_TrueGear.BuildInfo.Author, TheForest_TrueGear.BuildInfo.DownloadLink)]
[assembly: MelonColor()]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame(null, null)]