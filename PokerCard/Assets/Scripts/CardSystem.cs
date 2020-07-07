using System.Collections.Generic;   // List
using System.Collections;           // 協程
using System.Linq;                  // 查詢
using UnityEngine;
using UnityEngine.UIElements;

// 在編輯模式執行
[ExecuteInEditMode]
public class CardSystem : MonoBehaviour
{
    /// <summary>
    /// 撲克牌：所有撲克牌，52張
    /// </summary>
    public List<GameObject> cards = new List<GameObject>();

    /// <summary>
    /// 花色： 黑桃、方塊、愛心與梅花
    /// </summary>
    private string[] type = { "Spades", "Diamond", "Heart", "Club" };

    private void Start()
    {
        GetAllCard();
    }

    /// <summary>
    /// 取得所有撲克牌
    /// </summary>
    private void GetAllCard()
    {
        if (cards.Count == 52) return;          // 如果 撲克牌.數量 等於 52 跳出

        // 跑四個花色
        for (int i = 0; i < type.Length; i++)
        {
            // 跑1-13張
            for (int j = 1; j < 14; j++)
            {
                string number = j.ToString();   // 數字 = j.轉字串
                switch (j)
                {
                    case 1:                     // 數字 1 改為 A
                        number = "A";
                        break;
                    case 11:                    // 數字 11 改為 A
                        number = "J";
                        break;
                    case 12:                    // 數字 12 改為 A
                        number = "Q";
                        break;
                    case 13:                    // 數字 13 改為 A
                        number = "K";
                        break;
                }
                // 卡牌 = 素材.載入<遊戲物件>("素材名稱")
                GameObject card = Resources.Load<GameObject>("PlayingCards_" + number + type[i]);
                // 撲克牌.添加(卡牌)
                cards.Add(card);
            }
        }
    }

    public void ChooseCardByType(string type)
    {
        DeleteAllChild();               // 刪除所有子物件
        // 暫存牌組 = 撲克牌.哪裡((物件) => 物件.名稱.包含(花色))
        var temp = cards.Where((x) => x.name.Contains(type));


        // 迴圈 遍尋 暫存牌組 生成(卡牌,父物件)
        foreach (var item in temp) Instantiate(item, transform);

        StartCoroutine(SetChildPosition());         // 啟動協程

    }

    /// <summary>
    /// 洗牌
    /// </summary>
    public void Shuffle()
    {
        DeleteAllChild();

        /// 參考型別 - ToArray() 轉為陣列實值型別 ToList() 轉會清單
        List<GameObject> shuffle = cards.ToArray().ToList();            // 另存一份洗牌用的原始牌組
        List<GameObject> newCards = new List<GameObject>();             // 儲存洗牌後的牌組
        for (int i = 0; i < 52; i++)
        {
            int r = Random.Range(0, shuffle.Count);                         // 從洗牌用牌組隨機挑一張

            GameObject temp = shuffle[r];                                   // 挑出來的隨機卡牌
            newCards.Add(temp);                                             // 添加到新牌組
            shuffle.RemoveAt(r);                                            // 刪除挑出來的牌


        }
        foreach (var item in newCards) Instantiate(item, transform);

        StartCoroutine(SetChildPosition());
    }
    public void Sort()
    {
        DeleteAllChild();

        // 排序後的卡牌 = 從cards找資料放到card
        // where 條件 - 梅花 || 愛心 || 方塊|| 黑桃
        // select 選取 card
        var sort = from card in cards
                   where card.name.Contains(type[3]) || card.name.Contains(type[2]) || card.name.Contains(type[1]) || card.name.Contains(type[0])
                   select card;

        foreach (var item in sort) Instantiate(item, transform);
   
        StartCoroutine(SetChildPosition());
    }

    private void DeleteAllChild()
    {
        for (int i = 0; i < transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);
    }
    /// <summary>
    /// 設定子物件坐標：排序、大小、角度
    /// </summary>
    private IEnumerator SetChildPosition()
    {
        yield return new WaitForSeconds(0.1f);              // 避免刪除這次的卡牌
        for (int i = 0; i < transform.childCount; i++)      // 迴圈執行每一個子物件
        {
            Transform child = transform.GetChild(i);        // 取得子物件
            child.eulerAngles = new Vector3(180, 0, 0);     // 設定角度
            child.localScale = Vector3.one * 20;            // 設定尺寸

            // x = i % 13 讓每13張都從1開始
            // x = (i - 6) * 間距
            float x = i % 13;
            int y = i / 13;
            child.position = new Vector3((x - 6) * 1.3f, 4 - y*2, 0);

            yield return null;
        }
    }
}
