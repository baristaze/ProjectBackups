package net.pic4pic.ginger.entities;

public enum Gender implements IntegerEnum {
	
	Unknown(0),
	Male(1),
	Female(2);
	
	private final int value;
	
	private Gender(int value) {
		this.value = value;
	}

	public int getIntValue() {
		return this.value;
	}
}
