using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
enum ConsoleCommand
{
    Spawn,
    Destroy,
    MapAllTiles,
    Tp,
    Null,
    LevelUp,
}
public class CheatConsole : Singleton<CheatConsole>
{
    public TMP_InputField consoleInputField;
    public TextMeshProUGUI oldInput;
    public string currentInput;
    public List<string> prevousInputs;
    private int PINum;

    public void ScrollUp()
    {

        {

            if (prevousInputs.Count > 0)
            {

                PINum++;
                if (PINum >= prevousInputs.Count) PINum = prevousInputs.Count - 1;
                consoleInputField.text = prevousInputs[PINum];
            }
        }
    }
    
    public void ScrollDown()
    {
        {

            if (prevousInputs.Count > 0)
            {

                PINum--;
                if (PINum < 0) PINum = 0;
                consoleInputField.text = prevousInputs[PINum];
            }
        }
    }
    public void Submit(string s)
    {
       
        if(s != "")
        {
            currentInput = s;
            ProcessInput();
            ClearInput();
            consoleInputField.ActivateInputField();
        }
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow)) ScrollUp();
        if(Input.GetKeyDown(KeyCode.DownArrow)) ScrollDown();
    }

    private void ClearInput()
    {
        if (currentInput != "")
        {
            if (prevousInputs.Count == 0 || prevousInputs[prevousInputs.Count - 1] != currentInput)
            {
                prevousInputs.Add(currentInput);
            }
               
            PINum = prevousInputs.Count;
            consoleInputField.text = "";
        }
    }


    private void ProcessInput()
    {
        if (currentInput.Length > 0)
        {
            oldInput.text += "\n " + currentInput;
            string[] inputParts = currentInput.Split('.');
            foreach (ConsoleCommand c in ConsoleCommand.GetValues(typeof(ConsoleCommand)))
            {

                if (inputParts[0].ToLower() == c.ToString().ToLower())
                {
                    ProcessCommand(c, inputParts);
                    return;
                }
            }
            ThrowWrongInput();
            
        }
    }
    private void ProcessCommand(ConsoleCommand command, string[] inputParts)
    {
        switch (command)
        {
            case ConsoleCommand.Spawn: Spawn(inputParts);
                break;
            case ConsoleCommand.Destroy: 
                break;
            case ConsoleCommand.LevelUp: LevelUp(inputParts);
                break;
            case ConsoleCommand.MapAllTiles: MapAllTiles();
                break;
            case ConsoleCommand.Null:
                break;
        }



    }
    private void Spawn(string[] inputParts)
    {
        if (Selector.selectedTile != null)
        {
            if (Selector.selectedTile != PlayerAbilitiesSystem.CurrentTile)
            {

                if (ItemDataBase.ObjectsByName.ContainsKey(inputParts[1].ToLower()))
                {
                    ObjectData data = ItemDataBase.GetObject(inputParts[1]);
                    if(inputParts.Length > 2)
                    {
                        if (inputParts[2].ToLower() == "random")
                        {
                            data = ObjectDataFactory.GetRandomCreature(MapGenerator.i.location.level, Biome.Any, null, true, inputParts[1]);
                        }
                    }


                    Spawner.Spawn(data, Selector.selectedTile.position);
                    newLine("Spawned " + inputParts[1], "green");
                }
                else
                {
                    newLine("There is no such object!", "red");

                }
            }
            else
            {
                newLine("You can not spawn objects on the tile player standing", "red");
            }
        }
        else
        {
            newLine("Select a tile first!", "red");
        }
    }
    private void LevelUp(string[] inputParts)
    {
        if(TryParseToFloat(inputParts[1] , out float levels))
        {
            for (int i = 0; i < levels; i++)
            {
                XPCounter.AddXP(XPCounter.nextLevelCost);
            }
            newLine("Added " + levels + " xls ", "green");
        }
        else
        {
            ThrowWrongInput();
        }
    }
    private void Suicide()
    {
        PlayerAbilitiesSystem.playerEntity.GetComponentData<CreatureComponent>().ChangeHealth(-Int32.MaxValue);

    }
    private void Tp()
    {
        if (Selector.selectedTile != TileData.Null)
        {


        }
    }
    private void ChangeTimescale(string[] inputParts)
    {
        float scale = 0;
        if (TryParseToFloat(inputParts[0], out scale))
        {
            Engine.i.timescale = scale;
            newLine("Timescale changed to " + scale, "green");
        }
        else
        {
            ThrowWrongInput();
        }
    }
    void MapAllTiles()
    {
        var tiles = TileUpdater.current;
        for (int i = 0; i < tiles.Length; i++)
        {
            var tile = tiles[i];
            tile.visible = true;
            tile.maped = true;
            tile.updateScheduled = true;
            tile.L = 0.8f;
            tile.Save();
                
        }

        TileUpdater.UpdateTileVisibility();

    }
   
    private bool TryParseToFloat(string parseString, out float result)
    {
        if(float.TryParse(parseString, out result));
        {
            return true;
        }
        return false;

    }
    private void newLine(string line, string color)
    {
        oldInput.text += "\n  <color=" + color + ">" + line + "</color>";
    }
    private void ThrowWrongInput()
    {
        newLine("Wrong input", "red");
    }
}
