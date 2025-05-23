using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ExamAnswer { Congruent=1, Incongruent }
public enum ExamInput {None, Congruent, Incongruent }
public enum Gender { Boy,Girl }
public enum ExamType { None, Car, FlankerFish, Memory, Switcher, Giraffe , Stroop  }
public enum ExamRunType { Practice, Exam }

[Serializable]
public class UserInfo
{
    public long id;
    public int colorId;
    public string name;
    public Gender gender;
    public string age;
    public DateTime userCreateDate;
    //采样负责人的名字
    public string ptName;
}

[Serializable]
public class UserExamData
{
    public UserInfo user;
    public List<ExamInfo> lst_examInfos;

}

[Serializable]
public class ExamInfo
{
    public long id;
    public ExamType examType;
    public DateTime dateTime;
    public List<ExamRecord> lst_examRecords = new List<ExamRecord>();
}

[Serializable]
public class OptionInfo
{
    public int soundVolume;
    public List<ExamOption> lst_examOptions = new List<ExamOption>();
}

[Serializable]
public class ExamRecord
{
    public int blockId;
    public string sessionType;
    public ExamAnswer examAnswer;
    public ExamInput examInput;
    public float inputDelay;
    public bool isTimeOut;
}

[Serializable]
public class ExamOption
{
    public ExamType examType;
    
    /// <summary>
    /// 最大6，代表3秒
    /// </summary>
    public int practiceTimeLimit;
    public int practiceRound;

    public int passScore;

    /// <summary>
    /// 最大6，代表3秒
    /// </summary>
    public int examTimeLimit;
    public int examRound;

    public ExamOption() { }
    public ExamOption(ExamType examType) 
    { 
        this.examType = examType;
        practiceTimeLimit = 4;
        practiceRound = 10;
        passScore = 75;
        examTimeLimit = 2;
        examRound = 20;

    }
}


