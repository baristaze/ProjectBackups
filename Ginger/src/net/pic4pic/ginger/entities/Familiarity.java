package net.pic4pic.ginger.entities;

public enum Familiarity implements IntegerEnum {
	Stranger(0),
	Familiar(1);
	
    private final int value;
	
	private Familiarity(int value) {
		this.value = value;
	}

	public int getIntValue() {
		return this.value;
	}
}
