using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LevelModel {

    public int level;
    public int move;
    public int target;
    public int collected;
    public Vector2Int size;
    public List<int> pieces;

    public List<PieceModel> init_piece;
    public List<ItemModel> init_item;
    public List<ObstacleModel> init_obstacle;

    public LevelModel(int p_nLevel, Vector2Int p_v2iSize) {
        level = p_nLevel;
        size = p_v2iSize;
        pieces = new List<int>();

        init_piece = new List<PieceModel>();
        init_item = new List<ItemModel>();
        init_obstacle = new List<ObstacleModel>();
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