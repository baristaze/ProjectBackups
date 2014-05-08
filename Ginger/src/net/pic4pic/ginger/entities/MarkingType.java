package net.pic4pic.ginger.entities;

public enum MarkingType implements IntegerEnum {
	
	Undefined(0),
	Viewed(1),
	Liked(2);
	
    private final int value;
	
	private MarkingType(int value) {
		this.value = value;
	}

	public int getIntValue() {
		return this.value;
	}
}
