/*
 * Created by SharpDevelop.
 * User: root
 * Date: 8/10/2008
 * Time: 12:48 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace ScantronMulti
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			runProgram();
		}
		
		
		void runProgram()
		{
			
			try{
				OpenFileDialog open = new OpenFileDialog();
				if (open.ShowDialog() == DialogResult.OK)
				{
					bool key = false;
					string[] keys = new string[1];
					if (MessageBox.Show("Would you like to compare to an answer key?\n\nYour key should be a single line on comma separated values, each a a string of numbers representing the correct answers with no spaces in between", "Key?", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						
						OpenFileDialog open2 = new OpenFileDialog();
						if (open2.ShowDialog() == DialogResult.OK)
						{
							StreamReader keyStream = new StreamReader(open2.FileName);
							key = true;
							keys = keyStream.ReadLine().Split(',');
							keyStream.Close();
						}
					}
					
					StreamReader inFile = new StreamReader(open.FileName);
					StreamWriter outFile = new StreamWriter(open.FileName + ".output.csv");
					
					string headers = "Last Name, First Name, Student ID";
					
					int length = 101;
					if (key){
						length = keys.Length+1;
					}
					
					for (int i = 1; i <length; i++)
					{
						headers += ", Q"+i.ToString();
						if (key)
						{
							headers += ", Q"+i.ToString()+" - Possible";
							headers += ", Q"+i.ToString()+" - Correct";
							headers += ", Q"+i.ToString()+" - Incorrect";
							headers += ", Q"+i.ToString()+" - Score";
						}
					}
					
					if (key)
					{
						headers += ", Cumulative Score";
					}
					
					outFile.WriteLine(headers);
					
					string line;
					string answer;
					string guess;
					int possible;
					int correct;
					int incorrect;
					float score;
					float cumScore;
					do
					{
						line = inFile.ReadLine();
						string split = line.Substring(0,12) + ", " + line.Substring(12,9)  + ", " +  line.Substring(21,9);
						int start;
						cumScore = 0;
						for (int i = 0; i <length; i++)
						{
							start = 30 + i*5;
							guess = line.Substring(start,5).Replace(" ", "");
							split += ", "+ guess;
							if (key && i < keys.Length)
							{
								//get key
								answer = keys[i];
								//possible
								possible = answer.Length;
								//correct
								correct = 0;
								for(int ii =0; ii<possible; ii++)
								{
									if (guess.Contains(answer[ii].ToString())){
										correct ++;
									}
								}
								//incorrect
								incorrect = guess.Length - correct;
								//score
								score = (float)(correct - incorrect);
								if (score < 0){score = 0;}
								score = (score / possible) * 2;
								cumScore += score;
								//print
								split += ", " + possible.ToString();
								split += ", " + correct.ToString();
								split += ", " + incorrect.ToString();
								split += ", " + score.ToString();
							}
						}
						
						split += cumScore.ToString();
						
											
						outFile.WriteLine(split);
//						if(inFile.EndOfStream == true)
//						{
//							MessageBox.Show(line);
//							MessageBox.Show(split);
//						}
					}
					while(inFile.EndOfStream == false);
					inFile.Close();
					outFile.Close();
				}
				
				
				MessageBox.Show("A new file has been created with your ouput: "+open.FileName + ".output.csv");
			}
			catch(Exception e)
			{
				MessageBox.Show("PROGRAM FAILED. \n\nERROR MESSAGE:\n" + e.ToString());
			}
			
			Environment.Exit(0);
			
		}
		
		
		
		
		
	}
}
