using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public static class CommandManager
{
    public static void ExecuteCommand(string command)
    {
        if (command == null || command.Length == 0)
            return;

        //do some sanitizations
        string[] commands = command.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < commands.Length; i++)
            ProcessCommand(commands[i].Trim('\n'));
    }
    static void ProcessCommand(string command)
    {
        //commands come in parts, generally
        //ACTION_TYPE_TYPEOFTYPE_PARAMETERS
        string[] parts = command.Split(' ');

        //process what type of command
        switch (parts[0])
        {
            case "modify":
                switch (parts[1])
                {
                    case "fleet":
                        ModifyFleet(parts);
                        break;
                    case "vital":
                        ModifyVital(parts);
                        break;
                    case "progress":
                        ModifyProgress(parts);
                        break;
                }
                break;

            case "add":
                {
                    switch(parts[1])
                    {
                        case "modifier":
                            AddModifier(parts);
                            break;
                    }

                    break;
                }

            case "evaluate":
                switch (parts[1])
                {
                    case "vital":
                        EvaluateVital(parts);
                        break;
                }
                break;

            case "load":
                Load(parts);
                break;

            case "generatesystem":
                GenerateNewSystem();
                break;

            default:
                ConsoleManager.Print("<color=red>Unknown Command</color> >> " + parts[0] + " :(");
                break;
        }
    }

    static void Load(string[] parts)
    {
        switch (parts[1])
        {
            case "overworld":
                ConsoleManager.Print("Loading overworld.");

                if (SceneManager.GetActiveScene().name == "Main")
                    ConsoleManager.Print("<color=red>Warning</color>: Loading overworld from main menu will break the game.");

                SceneManager.LoadScene("Overworld");
                break;
            case "event":
                LoadEvent(parts);
                break;
            default:
                ConsoleManager.Print("<color=red>" + parts[1] + " is not a valid scene/event.</color>");
                break;
        }
    }
    static void LoadEvent(string[] parts)
    {
        DialogueEvent de = null;

        //if is id
        if (Regex.IsMatch(parts[2], @"^\d+$"))
            de = EventDB.GetByID(int.Parse(parts[2]));
        else
            de = EventDB.GetByName(parts[2]);

        if (de != null)
        {
            ConsoleManager.Print("Loading event " + de.name);
            DialogueUIManager.getInstance.DisplayDialogueEvent(de);
        }
        else
            ConsoleManager.Print("<color=red>Event " + parts[2] + " does not exist.</color>");
    }
    static void GenerateNewSystem()
    {
        ConsoleManager.Print("Generating new system.");
        RuntimeData.GenerateNewSystem();
    }

    static void ModifyFleet(string[] parts)
    {
        switch (parts[1])
        {
            case "add":
                break;
            case "addrandom":
                break;

            case "remove":
                Ship sh = PlayerData.fleet.ships
                    .FirstOrDefault(s => s.name == parts[2]);

                if (sh != null)
                    PlayerData.fleet.RemoveShip(sh);
                break;
            case "removerandom":
                PlayerData.fleet.RemoveShip(PlayerData.fleet.ships[Random.Range(0, PlayerData.fleet.ships.Count)]);
                break;

            default:
                ConsoleManager.Print("<color=red>Unknown Command</color> >> " + parts[0] + " :(");
                break;
        }
    }
    static void ModifyVital(string[] parts)
    {
        if(parts[2] == "percentage")
        {
            float percentage = float.Parse(parts[4]) * .01f;
            FleetVital v = PlayerData.fleet.GetVital((FleetVitalType)Enum.Parse(typeof(FleetVitalType), parts[3], true));

            v.Update(v.current * percentage);
        }
        else
            PlayerData.fleet
                .GetVital((FleetVitalType)Enum.Parse(typeof(FleetVitalType), parts[2], true))
                .Update(float.Parse(parts[3]));
    }
    static void ModifyProgress(string[] parts)
    {
        ConsoleManager.Print("Setting " + (ProgressPoint)Enum.Parse(typeof(ProgressPoint), parts[2], true) + " to " + bool.Parse(parts[3]));
        ProgressData.Set((ProgressPoint)Enum.Parse(typeof(ProgressPoint), parts[2], true), bool.Parse(parts[3]));
    }

    //0 - add
    //1 - modifier
    //2 - id/random/all
    //3 - civilian/military/all
    //4 - value
    //5 - additive/percentage
    //6 - current/max
    //7 - static/repeating
    //8 - fleetvitaltype
    //9 - duration (< 0 for infinite)
    //10 - reason (string)
    static void AddModifier(string[] parts)
    {
        bool random = parts[2].Equals("random");

        Ship[] ships = PlayerData.fleet.ships.ToArray();

        if (random)
        {
            
        }
        else
        {
            
        }
    }

    public static bool EvaluateOption(DialogueOption option)
    {
        if (option.conditions == null || option.conditions.Length == 0)
            return true;

        string[] conditions = option.conditions.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < conditions.Length; i++)
        {
            if (!EvaluateVital(conditions[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)))
                return false;
        }

        return true;
    }
    static bool EvaluateVital(string[] parts)
    {
        return PlayerData.fleet.GetVital((FleetVitalType)Enum.Parse(typeof(FleetVitalType), parts[2], true)).current >= float.Parse(parts[3]);
    }

    public static string ProcessFlags(string original)
    {
        //FORMATVITAL_VITAL
        foreach (string word in original.Split(new char[] { ' ', '.', ',', '!', '?', '\n' }))
        {
            if (word.Contains("FORMATVITAL"))
            {
                original = Regex.Replace(original, word, FleetVital.Format((FleetVitalType)Enum.Parse(typeof(FleetVitalType), word.Split('_')[1], true)));
            }
            else if (word.Contains("LISTPLAYERFLEET"))
            {
                //LISTPLAYERFLEET
                //generate list of player fleet
                string fleet = "";

                for (int i = 0; i < PlayerData.fleet.ships.Count; i++)
                    fleet += "the <i><color=yellow>" + PlayerData.fleet.ships[i].name + "</color></i>" + (i == PlayerData.fleet.ships.Count - 1 ? "" : (i == PlayerData.fleet.ships.Count - 2 ? " and " : ", "));

                original = Regex.Replace(original, @"\bLISTPLAYERFLEET\b", fleet);
            }
            else if (word.Contains("LISTFACTIONMODIFIERS"))
            {
                ////LISTFACTIONMODIFIERS
                //string modifiers = "";

                //for (int i = 0; i < RuntimeData.save.data.player.modifiers.Length; i++)
                //    modifiers += RuntimeData.save.data.player.modifiers[i].GetTooltip() + "\n";

                //original = Regex.Replace(original, @"\bLISTFACTIONMODIFIERS\b", modifiers);
            }
        }

        return original;
    }
}