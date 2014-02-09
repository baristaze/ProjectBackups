package net.pic4pic.ginger.entities;

public enum NotificationAction implements IntegerEnum {
	None(0),
	PokeBack(1),
	LikeBackBio(2),
	ViewProfile(3),
	ViewMessage(4),
	RequestP4P(5),
	AcceptP4P(6);
	
    private final int value;
	
	private NotificationAction(int value) {
		this.value = value;
	}

	public int getIntValue() {
		return this.value;
	}
}
