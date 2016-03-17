﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;

namespace LevelBuilder
{
	public class TileGridEditor : MonoBehaviour 
	{
		public GameObject buttonTilesContainer;
		public ButtonTile normalButtonTile;
		public Image selectedToShow;
		public GameObject gridTilesContainer;
		private ButtonTile currentButtonSelected;
		private ButtonTile[] buttons;
		private GridTile[] gridTiles;

		private bool initialized = false;

		void Start()
		{
			if(!initialized)
			{
				normalButtonTile.findSpriteToShow();

				buttons = buttonTilesContainer.GetComponentsInChildren<ButtonTile>(true);

				foreach(ButtonTile btn in buttons)
				{
					btn.findSpriteToShow();
					btn.onClickNotification += OnButtonClick;
				}

				setCurrentButtonSelected(normalButtonTile);

				gridTiles = gridTilesContainer.GetComponentsInChildren<GridTile>(true);

				foreach(GridTile tile in gridTiles)
				{
					tile.onClick+= onTileClick;
					setTileValueWithButton(tile,normalButtonTile);
				}
			}

			initialized = true;
		}

		public void Inititalize()
		{
			Start();
		}

		public void OnButtonClick(ButtonTile target)
		{
			setCurrentButtonSelected(target);
		}

		private void setCurrentButtonSelected(ButtonTile current)
		{
			currentButtonSelected = current;
			selectedToShow.sprite = current.spriteToshow;
		}

		public void onTileClick(GridTile target)
		{
			if(gameObject.activeInHierarchy)
			{
				setTileValueWithButton(target,currentButtonSelected);
			}
		}

		public void onSetAllTiles()
		{
			for(int i=0; i<gridTiles.Length; i++)
			{
				setTileValueWithButton(gridTiles[i],currentButtonSelected);
			}
		}

		private void setTileValueWithButton(GridTile tile, ButtonTile button)
		{
			tile.setDataValue(button.dataValue);
			tile.setSpriteToShow(button.spriteToshow);
		}

		public void sincronizeDataWithCSV(string csv)
		{
			resetDataItems();

			if(csv.Length == 0){return;}

			string[] tiles = csv.Split(',');
			ButtonTile tempBtn;

			for(int i = 0; i < tiles.Length; i++)
			{
				if(i >= gridTiles.Length)
				{
					Debug.LogError("La cantidad de tiles indicados para la grid supera los existentes.");
					return;
				}

				tempBtn = getButtonTileWithValue(int.Parse(tiles[i]));

				if(tempBtn == null)
				{
					tempBtn = normalButtonTile;
				}

				gridTiles[i].setDataValue(tempBtn.dataValue);	
				gridTiles[i].setSpriteToShow(tempBtn.spriteToshow);
			}
		}

		private ButtonTile getButtonTileWithValue(int value)
		{
			foreach(ButtonTile btn in buttons)
			{
				if(btn.dataValue == value)
				{
					return btn;
				}
			}

			return null;
		}

		public string getCSVData()
		{
			StringBuilder builder = new StringBuilder();

			for(int i = 0; i < gridTiles.Length; i++)
			{
				if(builder.Length != 0)
				{
					builder.Append(",");
				}

				builder.Append(gridTiles[i].dataValue.ToString());
			}

			return builder.ToString();
		}

		public void resetDataItems()
		{
			foreach(GridTile tile in gridTiles)
			{
				tile.setDataValue(normalButtonTile.dataValue);
				tile.setSpriteToShow(normalButtonTile.spriteToshow);
			}
		}
	}
}