using StardewValley;

namespace StardewBotFramework.Source.Events.EventArgs;

public class HUDMessageAddedEventArgs
{

	public string Message;
	public string Type;
	public int WhatType;
	public int SubjectNumber;
	public bool Achievement;
	public bool NoIcon;
	public Item MessageSubjectItem;
	
	public HUDMessageAddedEventArgs(string message,string type,int whatType,int subjectNumber,bool achievement,bool noIcon,Item messageSubject)
	{
		Message = message;
		Type = type;
		WhatType = whatType;
		SubjectNumber = subjectNumber;
		Achievement = achievement;
		NoIcon = noIcon;
		MessageSubjectItem = messageSubject;
	}

}