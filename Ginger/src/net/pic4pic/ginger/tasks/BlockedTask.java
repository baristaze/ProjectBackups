package net.pic4pic.ginger.tasks;

import net.pic4pic.ginger.R;
import android.app.ProgressDialog;
import android.content.Context;
import android.os.AsyncTask;
import android.widget.Button;

public abstract class BlockedTask<ExecArgType, X, ResultType> 
	extends AsyncTask<ExecArgType, X, ResultType> {
	
	protected Context context;
	protected ProgressDialog progress; 
	protected Button button;
	protected boolean showProgressBar;
	
	protected BlockedTask(Context context){
		this(context, null);
	}
	
	protected BlockedTask(Context context, Button button){
		this(context, button, true);
	}
	
	protected BlockedTask(Context context, Button button, boolean showProgressBar){
		this.context = context;
		this.button = button;
		this.showProgressBar = showProgressBar;
	}
	
	@Override
	protected void onPreExecute() {
		if(this.showProgressBar){
			this.progress = new ProgressDialog(this.context);
			this.progress.setTitle(this.context.getString(R.string.general_processing));
			this.progress.setMessage(this.context.getString(R.string.general_please_wait));
			this.progress.setCancelable(false);
			this.progress.setIndeterminate(true);
			this.progress.show();
		}
		
		if(this.button != null){
			this.button.setEnabled(false);
		}
	}
	
	@Override
	protected void onPostExecute(ResultType result) {
		
		if (this.progress!=null) {
			this.progress.dismiss();			
		}
		
		if(this.button != null){
			this.button.setEnabled(true);
		}
	}
}
