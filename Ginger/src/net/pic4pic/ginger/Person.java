package net.pic4pic.ginger;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.UUID;

public class Person implements Serializable {
	
	private static final long serialVersionUID = 1;
	
	protected UUID id;
	protected String username;	
	protected String shortBio;
	protected String avatarUri;
	protected String description;
	protected String mainPhoto;
	protected Familiarity familiarity;
	protected Gender gender;
	
	protected ArrayList<ImageInfo> otherPhotos;
	
	/*
	protected int age;
	protected MaritalStatus maritalStatus;
	*/
	
	public Person(){
		this.familiarity = Familiarity.Stranger;
		this.otherPhotos = new ArrayList<ImageInfo>();
	}
	
	public UUID getId(){
		return this.id;
	}
	
	public void setId(UUID id){
		this.id = id;
	}
	
	public String getUsername(){
		return this.username;
	}
	
	public void setUsername(String username){
		this.username = username;
	}
	
	public String getShortBio(){
		return this.shortBio;
	}
	
	public void setShortBio(String shortBio){
		this.shortBio = shortBio;
	}
	
	public String getAvatarUri(){
		return this.avatarUri;
	}
	
	public void setAvatarUri(String avatarUri){
		this.avatarUri = avatarUri;
	}
	
	public String getDescription(){
		return this.description;
	}
	
	public void setDescription(String description){
		this.description = description;
	}
	
	public String getMainPhoto(){
		return this.mainPhoto;
	}
	
	public void setMainPhoto(String mainPhoto){
		this.mainPhoto = mainPhoto;
	}
	
	public Familiarity getFamiliarity(){
		return this.familiarity;
	}
	
	public void setFamiliarity(Familiarity familiarity){
		this.familiarity = familiarity;
	}
	
	public Gender getGender(){
		return this.gender;
	}
	
	public void setGender(Gender gender){
		this.gender = gender;
	}
	
	public ArrayList<ImageInfo> getOtherPhotos(){
		return this.otherPhotos;
	}
	
	@Override
	public String toString(){
		return this.username;
	}
}
