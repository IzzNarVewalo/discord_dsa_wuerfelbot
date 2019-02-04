using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using _04_dsa.Services;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Xml.Linq;

namespace _04_dsa.Modules {
    // Modules must be public and inherit from an IModuleBase
    public class PublicModule : ModuleBase<SocketCommandContext> {

        // Dependency Injection will fill this value in for us
        public PictureService PictureService { get; set; }
        public DBService DBService { get; set; }
        public Helden_software_API Helden_software_API { get; set; }
        public DsaModule DsaModule { get; set; }

        [Command("help")]
        [Alias("Help", "Hilfe", "Info", "hilfe", "info")]
        public async Task HelpAsync([Remainder] string command = "") {
            command = command.ToLower();
            var eb = new EmbedBuilder();
            Embed embed = null;
            switch (command) {
                case "!help":
                case "help":
                case "!hilfe":
                case "hilfe":
                    embed = eb.AddField("!help [BEFEHL]",
                        "Zeigt die Hilfe an. optional Hilfe für andere Befehle." +
                        "```python\n" +
                        "# Beispiel zur benutzung:\n" +
                        "!help Probe```")
                    .WithColor(Color.Blue)
                    .Build();
                    break;
                case "list":
                case "!list":
                    embed = eb.AddField("!list",
                        "listet alle Helden auf die momentan online hochgeladen wurden.\n" +
                        "In diese rollen kannst du mithilfe von **!impersonate** schlüpfen." +
                        "```python\n" +
                        "# Beispiel zur benutzung:\n" +
                        "!list```")
                    .WithColor(Color.Blue)
                    .Build();
                    break;
                case "impersonate":
                case "!impersonate":
                    embed = eb.AddField("!impersonate HELDENNAME",
                        "Hiermit schlüpfst du in die Rolle eines Helden.\n" +
                        "Während du die Rolle eines Helden übernommen hast, werden seine Eigenschaften, TaW, ZfW, etc. für deine Proben mithilfe von **!probe** übernommen.\n" +
                        "Um in die Rolle eines anderen Helden zu schlüpfen, benutze den Befehl mit dem gewünschten Heldennamen.\n" +
                        "Dabei vergisst der Bot automatisch welcher Held du zuvor warst.\n" +
                        "```python\n" +
                        "# Beispiel zur benutzung:\n" +
                        "!impersonate Wolf```")
                    .WithColor(Color.Blue)
                    .Build();
                    break;
                case "whoami":
                case "!whoami":
                case "werbinich":
                case "!werbinich":
                case "wer bin ich":
                case "!wer bin ich":
                    embed = eb.AddField("!whoami",
                        "Zeigt dir den Namen des Helden an dessen rolle du über **!impersonate** übernommen hast.\n" +
                        "Dabei werden für deine Proben die du mit **!probe** würfelst die Werte dieses Helden hergenommen.\n" +
                        "```python\n" +
                        "# Beispiel zur benutzung:\n" +
                        "!whoami```")
                    .WithColor(Color.Blue)
                    .Build();
                    break;
                case "attributo":
                case "!attributo":
                    embed = eb.AddField("!probe attributo [+ERSCHWERNIS] [-ERLEICHTERUNG] [+-ZfW]*",
                        "Würfelt eine Attributo Probe für dich.\n" +
                        "Dieser Zauber wird speziell behandelt da der letzte Eigenschaftswurf von der Ansage abhängig ist.\n" +
                        "```python\n" +
                        "# Beispiel zur benutzung:\n" +
                        "!probe Attributo CH\n" +
                        "!probe attributo kl +2```")
                    .WithColor(Color.Blue)
                    .Build();
                    break;
                case "ritualkenntnis":
                case "!ritualkenntnis":
                    embed = eb.AddField("!probe RITUALKENNTNIS [+ERSCHWERNIS] [-ERLEICHTERUNG] [+-ZfW]*",
                        "Würfelt eine Ritualkenntnis Probe für dich.\n" +
                        "Dieses Talent wird speziell behandelt da die Eigenschaftswürfe von der Nutzung abhängig sind.\n" +
                        "```python\n" +
                        "# Beispiel zur benutzung:\n" +
                        "!probe Gildenmagie KL/KL/IN\n" +
                        "!probe hexe IN/CH/CH +2```")
                    .WithColor(Color.Blue)
                    .Build();
                    break;
                case "probe":
                case "!probe":
                    embed = eb.AddField("!probe TALENT-/ZAUBERNAME [+ERSCHWERNIS] [-ERLEICHTERUNG] [+-ZfW]*",
                        "Würfelt eine Probe mit den Werten des Helden in dessen Rolle du mit **!impersonate** geschlüpft bist.\n" +
                        "Zusätzliche Erleichterungen/Erschwernisse lassen sich wie im Beispiel angeben.\n" +
                        "Durch Spezialisierungen, etc. lassen sich auch die ZfW/ZfP* direkt beeinflussen.\n" +
                        "während man mehrere Erleichterungen/Erschwernisse reinschreiben kann," +
                        "ist das verrechnen der zusätzlichnen ZfW (wie beim Beispiel Überreden) nur einmalig möglich.\n" +
                        "```python\n" +
                        "# Beispiele zur benutzung:\n" +
                        "!probe Überreden +3 +2* # 3er Erschwernis Kulturkunde, +2 ZfW Spezialisierung lügen\n" +
                        "!probe Schleichen +7\n" +
                        "!probe Flim Flam -4\n" +
                        "!probe Sternenkunde +2 +3 -1```")
                    .WithColor(Color.Blue)
                    .Build();
                    break;
                case "roll":
                case "würfel":
                case "dice":
                    embed = eb.AddField("!roll xxxWyyyy", "Würfelt mit **x** würfeln die von **1** bis **y** rollen können.\n" +
                        "```python\n" +
                        "# Beispiele zur benutzung:\n" +
                        "!roll 3W20\n" +
                        "!roll 2W6\n" +
                        "!roll 40W6```")
                        .WithColor(Color.Blue)
                        .Build();
                    break;
                case "check":
                case "isonline":
                    embed = eb.AddField("!check", "überprüft ob die helden-software server online sind.\n" +
                        "```python\n" +
                        "# Beispiele zur benutzung:\n" +
                        "!check```")
    .WithColor(Color.Blue)
    .Build();
                    break;
                case "":
                    embed = eb.AddField("Hilfe", "Werte in **CAPSLOCK** sind zu ersetzende variablen, werte in **[eckigen Klammern]** sind optionale angaben.")
                    .AddField("**!help [BEFEHL]**: ", "Diese anzeige, evtl. weitere Informationen zu BEFEHL.")
                    .AddField("**!list**: ", "Zeigt alle Helden an die momentan Online sind.\nIn diese rollen kannst du mithilfe von **!impersonate** schlüpfen.")
                    .AddField("**!impersonate HELDENNAME**:", "Du übernimmst die Rolle des ausgewählten Helden." +
                    "\ndamit merkt sich der Bot mit welchem Helden er für dich die Proben würfeln soll.")
                    .AddField("**!whoami**:", "zeigt dir die Rolle deines aktuellen Helden an.")
                    .AddField("**!Probe TALENT-/ZAUBERNAME [+-MODIFIKATION]**:", "würfelt mit deinem Helden die Probe.")
                    .AddField("**!roll xWy**:", "rollt **x** mal einen würfel von 1 bis **y**.")
                    .AddField("**!check**:", "überprüft ob die helden-software server online sind.")
                    .WithFooter(footer => footer.Text = "die möglichen Befehle für den Bot.")
                    .WithColor(Color.Blue)
                    .Build();
                    break;
                default:
                    embed = eb.AddField("Sorry!", "der Befehl **" + command + "** wurde nicht gefunden.\n" +
                        "gebe **!help** für eine Liste verfügbarer Befehle ein.")
                        .WithColor(Color.Red)
                        .Build();
                    break;
            }

            await ReplyAsync(null, false, embed);
        }

        [Command("list")]
        [Alias("List")]
        public async Task ListAsync() {
            var heldenliste = await Helden_software_API.GetHeroList();
            if (!heldenliste.Any()) {
                await ReplyAsync("helden-software ist zurzeit offline. es wird auf ein Backup zugegriffen.");
                heldenliste = await DBService.GetHeldenListe();
            }
            var eb = new EmbedBuilder();
            var embed = eb
                .AddField("Folgende Helden sind derzeit Online:", String.Join("\n", heldenliste))
                .WithColor(Color.Blue)
                .Build();
            await ReplyAsync(null, false, embed);
        }

        [Command("whoami")]
        [Alias("werbinich", "wer bin ich")]
        public async Task WhoAmIAsync() {
            var db = await DBService.GetHeldenName(Context.User.Id);
            var eb = new EmbedBuilder();
            Embed embed = null;
            if (!string.IsNullOrEmpty(db)) {
                embed = eb.AddField(Context.User.Username, "Du bist zurzeit in der Rolle von: **" + db + "**")
                    .WithColor(Color.Blue)
                    .Build();
            } else {
                embed = eb.AddField(Context.User.Username, "Du bist noch keinem Helden zugeordnet.\n" +
                    "Führe **!impersonate** aus um in die Rolle eines Helden zu springen.")
                    .WithColor(Color.Red)
                    .Build();
            }
            await ReplyAsync(null, false, embed);
        }

        [Command("roll")]
        [Alias("Roll", "würfel", "Würfel", "dice", "Dice")]
        public async Task RollAsync(string param) {
            var eb = new EmbedBuilder();
            Embed embed;
            string output = "";
            var countXboundary = Helper.FormatRoll(param);
            if (countXboundary.Key < 1 || countXboundary.Value < 1) {
                embed = eb.AddField("Fehler", param + " ist kein gültiges Format. Bitte benutze das Format **xxxWyyy** bzw. **xxxDyyy**. oder gebe **!help roll** für die Hilfe ein.\n")
                    .WithColor(Color.Red)
                    .Build();
            } else {
                int[] results = DsaModule.Dice(countXboundary.Key, countXboundary.Value);
                for (int i = 0; i < countXboundary.Key; i++) {
                    output += results[i];
                    if (i + 1 < countXboundary.Key)
                        output += ", ";
                }
                embed = eb.AddField(param, output)
                    .WithColor(Color.Green)
                    .Build();
            }
            await ReplyAsync(null, false, embed);
        }

        [Command("impersonate")]
        [Alias("Impersonate", "einspringen", "Einspringen", "Übernehmen", "übernehmen")]
        public async Task ImpersonateAsync(string name) {
            var eb = new EmbedBuilder();
            var heldenliste = await Helden_software_API.GetHeroBasics(name);
            if (!heldenliste.Any()) {
                await ReplyAsync("helden-software ist zurzeit offline. es wird auf ein Backup zugegriffen.");
                heldenliste = await DBService.GetHeldenIDUndName(name.ToLower());
            }
            if (heldenliste.Count > 1) {
                string tmp = "";
                int count = 0;
                foreach (var entry in heldenliste) {
                    tmp += entry.Value;
                    count++;
                    if (count < heldenliste.Count)
                        tmp += ", ";
                }
                var embed = eb.AddField("Fehler", "Es fangen **" + heldenliste.Count + "** Helden mit \"" + name + "\" an (" + tmp + "), " +
                    "bitte sei etwas genauer wessen Rolle du übernehmen möchtest.")
                    .WithColor(Color.Red)
                    .Build();
                await ReplyAsync(null, false, embed);
            } else if (heldenliste.Count < 1) {
                var embed = eb.AddField("Fehler", "Leider habe ich keinen Helden namens \"" + name + "\" gefunden, " +
                    "gebe **!list** für eine Liste aller Helden die im Moment online sind ein.")
                    .WithColor(Color.Red)
                    .Build();
                await ReplyAsync(null, false, embed);
            } else {
                await DBService.UpdateDsaLink(Context.User.Id, heldenliste.Keys.First());
                await DBService.UpdateHeroName(heldenliste.Keys.First(), heldenliste.Values.First());
                var embed = eb.AddField(Context.User.Username, "Du übernimmst jetzt die Werte von: **" + heldenliste.Values.First() + "**.")
                    .WithColor(Color.Green)
                    .Build();
                await ReplyAsync(null, false, embed);
            }

        }

        [Command("probe")]
        [Alias("Probe", "TaW", "ZfW", "taw", "zfw")]
        public async Task TrialAsync(params string[] args) {
            var eb = new EmbedBuilder();
            Embed embed = null;
            int mod = 0;
            int extrasternchen = 0;
            var talent = new List<string>();
            bool isStringDone = false;
            var special = new List<Eigenschaften>();
            _3_W_20_Throw wurf = null;

            // parse the input
            for (int i = 0; i < args.Length; i++) {
                // überprüft ob der Sonderfall Attributo eingetroffen ist
                if ("attributo".StartsWith(args[i].ToLower()))
                    if (args.Length < 2 || !Enum.IsDefined(typeof(Eigenschaften), args[i + 1].ToUpper())) {
                        embed = eb.AddField("Fehler", "Die Syntax ist nicht korrekt.\n" +
                            "Gib: **!help Attributo** für hilfe wie man Ritualkenntnis auswürfelt ein.")
                            .WithColor(Color.Red)
                            .Build();
                        await Context.Channel.SendMessageAsync(null, false, embed);
                        return;
                    } else {
                        talent.Add(args[i]);
                        Eigenschaften eg;
                        Enum.TryParse(args[i + 1].ToUpper(), out eg);
                        special.Add(Eigenschaften.KL);
                        special.Add(Eigenschaften.CH);
                        special.Add(eg);
                        isStringDone = true;
                    }
                // überprüft ob der Sonderfall Ritualkenntnis eingetroffen ist
                if ("ritualkenntnis".StartsWith(args[i].ToLower())
                    || "gildenmagie".StartsWith(args[i].ToLower()))
                    if ("ritualkenntnis".StartsWith(args[i].ToLower()) || args.Length < 2 || args[i + 1].Length != 8) {
                        embed = eb.AddField("Fehler", "Die Syntax ist nicht korrekt.\n" +
                        "Gib: **!help Ritualkenntnis** für hilfe wie man Ritualkenntnis auswürfelt ein.")
                        .WithColor(Color.Red)
                        .Build();
                        await Context.Channel.SendMessageAsync(null, false, embed);
                        return;
                    } else {
                        talent.Add(args[i]);
                        Enum.TryParse(args[i + 1].Split('/')[0].ToUpper(), out Eigenschaften eg);
                        special.Add(eg);
                        Enum.TryParse(args[i + 1].Split('/')[1].ToUpper(), out eg);
                        special.Add(eg);
                        Enum.TryParse(args[i + 1].Split('/')[2].ToUpper(), out eg);
                        special.Add(eg);
                        isStringDone = true;
                    }
                // überprüft ob sternchen manipuliert werden
                if ((args[i].StartsWith('+') || args[i].StartsWith('-')) && args[i].EndsWith('*')) {
                    extrasternchen = Convert.ToInt32(args[i].Remove(args[i].Length - 1));
                    isStringDone = true;
                    // überprüft ob es modifikationen gibt
                } else if (args[i].StartsWith('+') || args[i].StartsWith('-')) {
                    mod += Convert.ToInt32(args[i]);
                    isStringDone = true;
                    // überprüft ob der talentname vorbei ist
                } else if (!isStringDone) {
                    talent.Add(args[i]);
                }
            }
            string talentprobe = string.Join(" ", talent);
            
            var held = await DBService.GetHeldenID(Context.User.Id);
            if (special.Any()) {
                wurf = await Helden_software_API.GetHeroValues(held.Key, talentprobe, DBService, special);
            } else {
                wurf = await Helden_software_API.GetHeroValues(held.Key, talentprobe, DBService);
            }
            if (wurf.Modifikation == -999 && wurf.Eigenschaft1 == "" && wurf.Wert2 == 0) {
                await ReplyAsync("helden-software ist zurzeit offline. es wird auf ein Backup zugegriffen." +
                    "\nMöglicherweise besitzt du das Talent/den Zauber auch nicht.");
                if (special.Any()) {
                    wurf = Helden_software_API.ParseSpecialProbe(XDocument.Parse(await DBService.GetHeldenXML(held.Key)), talentprobe, special);
                } else {
                    wurf = Helden_software_API.ParseProbe(XDocument.Parse(await DBService.GetHeldenXML(held.Key)), talentprobe);
                }
            }
            wurf.Modifikation = mod;
            wurf.TaW += extrasternchen;
            if (wurf.Talent == "F"
                && wurf.Eigenschaft1 == "A"
                && wurf.Eigenschaft2 == "I"
                && wurf.Eigenschaft3 == "L"
                && wurf.Wert1 == 0 && wurf.Wert2 == 0 && wurf.Wert3 == 0) {
                embed = eb.AddField("Fehler", "kein Talent namens: **" + talentprobe + "** gefunden.\n" +
                    "hast du dich verschrieben oder Besitzt du das Talent nicht?")
                    .WithColor(Color.Red)
                    .Build();
                await Context.Channel.SendMessageAsync(null, false, embed);
                return;
            }

            string quali = "";
            string StarGrammar = "";
            string spezialisierung = "";

            if (extrasternchen != 0)
                spezialisierung = extrasternchen.ToString("+0;-#");

            var wurfergebnis = new _3_W_20_Score(wurf);

            eb.AddField(wurf.Talent, "TaW: **" + (wurf.TaW - extrasternchen) + spezialisierung + "**\n" +
                "modifiziert um: **" + wurf.Modifikation.ToString("+0;-#") + "**")
                .AddField(wurf.Eigenschaft1, "Eigenschaft: **" + wurf.Wert1 + "**\nWurf: **" + wurfergebnis.W1 + "**", true)
                .AddField(wurf.Eigenschaft2, "Eigenschaft: **" + wurf.Wert2 + "**\nWurf: **" + wurfergebnis.W2 + "**", true)
                .AddField(wurf.Eigenschaft3, "Eigenschaft: **" + wurf.Wert3 + "**\nWurf: **" + wurfergebnis.W3 + "**", true);
            switch (wurfergebnis.ergebnis) {
                case Ergebnis.nicht_bestanden:
                    embed = eb.AddField("Misslungen", "Schade **" + held.Value.Split(' ')[0] + "** du hast die Probe nicht geschafft.")
                    .WithColor(Color.Red)
                    .Build();
                    break;
                case Ergebnis.bestanden:
                    if (wurfergebnis.Qualität > wurfergebnis.Sternchen)
                        quali = "Die Qualität beträgt: **" + wurfergebnis.Qualität + "**";
                    if (wurfergebnis.Sternchen != 1)
                        StarGrammar = "e";
                    else
                        StarGrammar = "";
                    embed = eb.AddField("Geschafft!", "Glückwunsch **" + held.Value.Split(' ')[0] + "** du hast **" + wurfergebnis.Sternchen + "** Stern" + StarGrammar + " übrig!\n" + quali)
                    .WithFooter(footer => footer.Text = "die Qualität wird nur angezeigt wenn sie höher als die erreichten Sternchen ist.")
                    .WithColor(Color.Green)
                    .Build();
                    break;
                case Ergebnis.patzer:
                    embed = eb.AddField("Patzer", "Was für ein Pech **" + held.Value.Split(' ')[0] + "**! Du hast leider gepatzt.")
                    .WithColor(new Color(0x9400d3))
                    .Build();
                    break;
                case Ergebnis.krit:
                    if (wurfergebnis.Qualität > wurfergebnis.Sternchen)
                        quali = "Die Qualität beträgt: **" + wurfergebnis.Qualität + "**";
                    if (wurfergebnis.Sternchen != 1)
                        StarGrammar = "e";
                    else
                        StarGrammar = "";
                    embed = eb.AddField("Kritisch!", "Glückwunsch **" + held.Value.Split(' ')[0] + "** du hast gekrittet, es sind **" + wurfergebnis.Sternchen + "** Sterne übrig!\n" + quali)
                    .WithFooter(footer => footer.Text = "Vergiss deine SE nicht!")
                    .WithColor(Color.Gold)
                    .Build();
                    break;

            }
            await ReplyAsync(null, false, embed);
        }

        [Command("check")]
        [Alias("Check", "isonline", "IsOnline")]
        public async Task IsOnline()
            => await ReplyAsync("helden-software server sind derzeit **" + await Helden_software_API.IsOnline() + "**.");

    }
}
