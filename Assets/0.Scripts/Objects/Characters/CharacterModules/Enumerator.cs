//경험치
public enum ExperienceValue
{
    //농사,   낚시,     제작,         광질,          채집, 
    Farming, Fishing, Production, MineralQuality, collection,
    Length
}

//움직임
public enum MoveCheckType
{
    //멈춤, 점프, 걷기, 상호 작용,
    Idle, Jump, Walk, Rotate,
    //제작,       나무베기,    채집,     낚시,   호미질,   낫질,     광질,    물뿌리기,   칼질
    Production, CuttingWood, Gathering, Fishing, Hoeing, Sickling, Polishing, Watering, Cutting,
    Length
}

//상태
public enum Conditions
{
    //배고픔, 갈증, 환각, 
    Hunger, Thirst, Hallucination, isRunning,
    Length
}

public enum StatusEFFects
{
    //독,   화상, 둔화
    Poison, Burn, Slow,
    Length
}

//등급
public enum Grade
{
    //가죽, 철, 골드, 다이아몬드, 이리듐
    Normal, Silver, Gold, Diamond, Iridium,
    Length
}

//도구
public enum Tool
{
    Axe,             //도끼
    Pickax,          //곡괭이
    Hoe,             //괭이
    Hammer,          //망치
    Homi,            //호미
    FishingRod,      //낚시대    
    WateringCan,     //물뿌리개    
    Sickle,          //낫
    Knife,           //칼
    Bags,            //가방
    Length
}
//장비
public enum Equipment
{
    Hat,            //모자
    Consultation,   //상의    
    Bottom,         //하의
    Shoes,          //신발
    Accessories,    //액세서리
    Glove,          //장갑
    Length
}
public enum Resource
{
    Basic,      //기본자원
    Processed,  //가공자원
    Heal,       //치유자원
    Length
}
public enum edible//먹을 수 있는!
{
    Herb,       // 약초
    Livestock,  // 축산
    Seed,       //씨앗
    Crop,       //작물
    Material,   //재료
    Machinable, //가공가능한
    Completion, //완성
    Length
}
