using System;

[Serializable]
public class Roles
{
    public Role[] roles;
}

[Serializable]
public class Role
{
	public string role_name;
	public string name_jp;
	public bool isWolf;
	public bool isFox;
	public Camp camp;
	public FotuneTellerRes fotuneTellerRes;
	public MediumRes mediumRes;
	public DisplayAtNight displayAtNight;
	public NightAction nightAction;
	public MorningAction morningAction;
	public WhenPunishmented whenPunishmented;
	public WhenBited whenBited;
	public WhenFortuneTelled whenFortuneTelled;
	public WhenGuarded whenGuarded;
	public int defaultNum;
}

public enum Camp {
	citizen,
	werewolf,
	fox,
}

public enum FotuneTellerRes {
	citizen,
	wolf,
}

public enum MediumRes {
	citizen,
	wolf,
}

public enum DisplayAtNight {
	none,
	werewolf,
	mediumRes,
	yoko,
}

public enum NightAction {
	suspectWolf,
	biteToKill,
	fotuneTelling,
	seeMediumRes,
	guardOtherPeople,
}

public enum MorningAction {
	none,
	deliveryBread,
}

public enum WhenPunishmented {
	punishmented,
	destinyBondAnyone,
	destinyBondNotWolf,
}

public enum WhenBited {
	death,
	cannotBite,
	notDeath,
	killWolf,
	destinyBondWolf,
}

public enum WhenFortuneTelled {
	none,
	death,
	killFortuneTeller
}

public enum WhenGuarded {
	guraded,
	killGuarder
}
