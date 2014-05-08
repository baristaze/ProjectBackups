package net.pic4pic.ginger.entities;

public enum ObjectType implements IntegerEnum {
	
	Undefined(0),
	Notification(1),
	Profile(2);
	
    private final int value;
	
	private ObjectType(int value) {
		this.value = value;
	}

	public int getIntValue() {
		return this.value;
	}
}