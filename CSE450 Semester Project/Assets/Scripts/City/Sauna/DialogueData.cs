using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueNode
{
    public string text;
    public string choiceAText;
    public string choiceBText;
}

public static class SaunaDialogue
{
    public static List<DialogueNode> allNodes = new List<DialogueNode>
    {
        new DialogueNode
        {
            text = "\"What is the purpose of your life?\"",
            choiceAText = "\"I'm not sure, I work a lot, to work? To make something?\"",
            choiceBText = "\"Hmm... to play. To live? I don't know.\""
        },
        
        new DialogueNode
        {
            text = "Steam swirls around you both.\nPeaceful silence.",
            choiceAText = "Close your eyes",
            choiceBText = "Focus on a knot in the wood that looks like a dog face"
        },
        
        new DialogueNode
        {
            text = "\"I'm an archive, a collection of things. \nAren't all things collections?\"",
            choiceAText = "\"I guess every whole is made up of parts.\"",
            choiceBText = "\"Some things are definitely just one thing.\""
        },
        
        new DialogueNode
        {
            text = "\"Every file system is also an archive of sorts. Every thing in this world is a file system. \n Just one nested hierarchy after another...\"",
            choiceAText = "\"Haha, so true.\"",
            choiceBText = "\"What makes you say that?\""
        },
        
        new DialogueNode
        {
            text = "\"What do you think of working at the pizza parlor?\"",
            choiceAText = "\"I like it. Sometimes the work feels like a game.\"",
            choiceBText = "\"I like that it gets me money for stuff, like hats.\""
        },
        
        new DialogueNode
        {
            text = "\"What do you think of this world?\"",
            choiceAText = "\"It's alright. Every tree looks the same.\"",
            choiceBText = "\"It's beautiful.\""
        },
        
        new DialogueNode
        {
            text = "The archive creature stares at the ceiling.\n\"This is nice,\" they whisper.",
            choiceAText = "\"Yeah.\"",
            choiceBText = "Nod silently"
        },
        
        new DialogueNode
        {
            text = "\"Who are you?\"",
            choiceAText = "\"Umm, I don't know, I work at the pizza parlor.\"",
            choiceBText = "\"I'm me.\""
        },
        
        new DialogueNode
        {
            text = "\"What color are you?\"",
            choiceAText = "\"On the inside? Red. Or a warm purple.\"",
            choiceBText = "\"Can't you see?\""
        },
        
        new DialogueNode
        {
            text = "\"I love the sauna. You don't need to think about anything in particular. You can't do it wrong.\"",
            choiceAText = "\"I dunno, I feel awkward. Sometimes it feels like I do everything wrong.\"",
            choiceBText = "\"Mmm.\""
        },
        
    };
    
    public static string startText = "Steam hits your face. A wise archive creature tends the coals. \nYou feel relaxed, but unsure...";
    public static string startChoiceA = "Sit quietly";
    public static string startChoiceB = "\"What should I think about in here?\"";
    public static string endText = "You feel tougher about the heat.\n(+1 Cooking)";
}