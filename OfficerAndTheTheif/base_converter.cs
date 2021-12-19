using System;

public class Base_Converter
{
	private int ChToIn(char c)
    {
		string chrs = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!#$%=?*@,.-_";
		return chrs.IndexOf(c);
    }

	private char InToCh(int i)
	{
		string chrs = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!#$%=?*@,.-_";
		return chrs[i];
	}
	public int ToDec(string num, int bs)
	{
		int dec = 0;
		int p = num.Length;
		for (int i = 0; i < num.Length; i++)
		{
			p--;
			dec = (int)(dec + (ChToIn(num[i]) * Math.Pow(bs, p)));
		}
		return dec;
	}

	public string FromDec(int dec, int bs)
	{
		if(dec == 0)
        {
			return "0";
        }
		string num = "";

		while(dec != 0)
        {
			num = InToCh(dec % bs) + num;
			dec = dec / bs;
        }

		return num;
	}

}
