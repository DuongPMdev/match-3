using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LevelModel {

    public int level;
    public int map;
    public int move;
    public Vector2Int size;
    public List<int> pieces; 
    public List<TargetModel> targets;

    public List<PieceModel> init_piece;
    public List<ItemModel> init_item;
    public List<ObstacleModel> init_obstacle;

    public LevelModel(int p_nLevel, Vector2Int p_v2iSize) {
        level = p_nLevel;
        size = p_v2iSize;
        pieces = new List<int>();

        targets = new List<TargetModel>();
        init_piece = new List<PieceModel>();
        init_item = new List<ItemModel>();
        init_obstacle = new List<ObstacleModel>();
    }

}

[Serializable]
public class TargetModel {

    public string type;
    public int value;
    public int request_amount;
    public int collected;

    public TargetModel(string p_stype, int p_nValue, int p_nRequestAmount) {
        type = p_stype;
        value = p_nValue;
        request_amount = p_nRequestAmount;
        collected = 0;
    }

}

[Serializable]
public class PieceModel {

    public Vector2Int position;
    public int piece;

    public PieceModel(Vector2Int p_v2iPosition, int p_nPiece) {
        position = p_v2iPosition;
        piece = p_nPiece;
    }

}

[Serializable]
public class ItemModel {

    public Vector2Int position;
    public int piece;
    public string type;

    public ItemModel(Vector2Int p_v2iPosition, int p_nPiece, string p_sType) {
        position = p_v2iPosition;
        piece = p_nPiece;
        type = p_sType;
        if (type.Equals("rainbow") == true) {
            piece = 0;
        }
    }

}

[Serializable]
public class ObstacleModel {

    public Vector2Int position;
    public string type;

    public ObstacleModel(Vector2Int p_v2iPosition, string p_sType) {
        position = p_v2iPosition;
        type = p_sType;
    }

}



[Serializable]
public class UserModel {

    public int heart;
    public string add_heart_anchor;
    public int checkin_count;
    public string checkin_anchor;
    public int coin;
    public int booster_1;
    public int booster_2;
    public int booster_3;
    public int booster_4;
    public int max_level;
    public List<MissionProceedModel> mission_proceed;

    public UserModel() {
        heart = 5;
        add_heart_anchor = DateTime.UtcNow.ToBinary().ToString();
        checkin_count = 0;
        checkin_anchor = "";
        coin = 0;
        max_level = 1;
        booster_1 = 0;
        booster_2 = 0;
        booster_3 = 0;
        booster_4 = 0;
        mission_proceed = new List<MissionProceedModel>();
    }

}

[Serializable]
public class MissionConfigModel {

    public string mission_id;
    public string icon;
    public string description;
    public int request;
    public int coin_reward;

}

[Serializable]
public class MissionProceedModel {

    public string mission_id;
    public int proceeded;
    public bool rewarded;

    public MissionProceedModel(string p_sMissionID) {
        mission_id = p_sMissionID;
        proceeded = 0;
        rewarded = false;
    }

}