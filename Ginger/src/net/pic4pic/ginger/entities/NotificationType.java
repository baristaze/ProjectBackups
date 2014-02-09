package net.pic4pic.ginger.entities;

public enum NotificationType implements IntegerEnum {
	
	Undefined(0),
	ViewedProfile(1),
	Poked(2),
	SentText(3),
	LikedBio(4),
	LikedPhoto(5),
	RequestingP4P(6),
	AcceptedP4P(7);
	
    private final int value;
	
	private NotificationType(int value) {
		this.value = value;
	}

	public int getIntValue() {
		return this.value;
	}
}
