using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//スクリプトからUIのImageコンポーネントを操作します。
//その為、using UnityEngine.UI;で、スクリプトからUIの操作が出来るようにします。

public class RayController : MonoBehaviour {

	public GameObject diveCamera;
	//DiveCameraの位置からRayを発射させます
	//なので、まずGameObject型でpublicな変数としてdiveCamera変数を定義

	public GameObject buttonCollider;
	//ButtonオブジェクトのColliderをアタッチする為の変数です
	//今回はButtonのColliderを用いて、Rayの衝突判定を行うためです。

	public GameObject ui;
	//全てのUIオブジェクトの親であるHierarchyビュー上の「UI」をアタッチします。

	public Image buttonGauge;
	//今回変化させるのは、先ほど追加したピンク色のゲージを持つButtonGaugeです。なので、取得するようにしましょう。

	public int endPositionX = -4;
	//ゲージがどこまで伸びるのかについての定義をしておきましょう。
	//ゲージの位置がこのendPositionXの位置より大きくなった場合、
	//Buttonが押されたことにする処理を後ほど記述します。また、状況に応じて変えられるようにpublicにしておきましょう。

	public GameObject stage;
	//色を変えるオブジェクトStageオブジェクトを取得しています。

	public GameObject easyImageCollider;
	public GameObject normalImageCollider;
	public GameObject hardImageCollider;
	//RayとヒットさせたいオブジェクトeasyImageCollider、normalImageCollider、hardImageColliderを取得

	bool changeHard;
	bool changeNormal;
	bool changeEasy;
	//bool型の変数が、どのColliderを持つオブジェクトと衝突したかの情報を渡すものになります。

	float gaugeTime;
	//今回はButtonに目線を合わせている時間に応じて、
	//ピンク色のゲージを変更させます。
	//なのでgaugeTimeの変数を定義しています。時間はfloat値で表すのでfloat型で定義しています。

	Vector3 firstButtonGaugePosition;
	//初めピンク色のゲージがどこにあるのかについての変数firstButtonGaugePositionを定義しています。
	//この変数を利用することで、Buttonの位置から目線をそらした場合に、ピンク色のゲージの位置を初期位置に戻すことが可能となります。

	GameObject[] difficultyImages;
	//GameObject型のdifficultyImagesという名前の配列を定義
	//後にDiffcultyImageタグが付いた全てのUIオブジェクトの情報を入れます。

	void Start () {
		difficultyImages = GameObject.FindGameObjectsWithTag ("DifficultyImage");
		//GameObject.FindGameObjectsWithTag ("DifficultyImage");の記述で、
		//Hierarchyビュー上にあるDifficultyImageタグが設定されているオブジェクトを全て取得して、
		//difficultyImages配列に代入をしています。

		firstButtonGaugePosition = buttonGauge.rectTransform.localPosition;
		//目を離した瞬間にゲージの場所を初期位置に戻す必要があります。
		//なぜこの記述で戻るのか
	}

	void Update () {
		Ray ray = new Ray (diveCamera.transform.position, diveCamera.transform.forward);
		//Rayの生成
		//new Ray(Rayを生成する位置, Rayを飛ばす方向);

		RaycastHit hit;
		//RayCasthit型を利用すれば、Rayを飛ばした際に衝突したオブジェクトの情報を得ることができます。

		if (Physics.Raycast (ray, out hit)) {
			//Physics.Raycast関数は
			//Rayがオブジェクトのコライダーと衝突した場合はtrue、それ以外はfalseを返します。
			//第一引数にRay型の値を指定し、
			//第二引数にRayが衝突した際のオブジェクトの情報を取得するためにRayCastHit型の値を指定します。
			//第三引数はRayを飛ばす距離を制限したい場合にfloat値を指定します。
			Debug.DrawLine (ray.origin, hit.point, Color.black);
			//Debug.DrawLineは、指定した開始位置と終了位置の間にラインを描画
			//Debug.DrawLine(ラインの開始位置, ラインの終了位置, ラインの色);

			if (hit.collider.gameObject.tag == "DifficultyCollider") {
				//「もし、Rayがヒットしたオブジェクトのタグの名前がDifficultyColliderだったら」
				for (int i = 0; i < difficultyImages.Length; i++) { 
					//difficultyImages配列の中の要素の数分だけ繰り返す
					difficultyImages [i].GetComponent<Image> ().color = Color.white;
					//.GetComponent<Image> ().colorでそれぞれのオブジェクトのImageコンポーネントにアクセス
				}
				hit.collider.gameObject.transform.parent.GetComponent<Image> ().color = Color.red;
				//「ヒットしたオブジェクトの親のImageの色を赤色にする」


				//↓RayがどのImageColliderと衝突したのかによって処理を変更させるスクリプト
				if (hit.collider.gameObject == hardImageCollider) {
					changeHard = true;
					print ("hard");
				}  else {
					changeHard = false;
				}

				if (hit.collider.gameObject == normalImageCollider) {
					changeNormal = true;
					print ("normal");
				}  else {
					changeNormal = false;
				}

				if (hit.collider.gameObject == easyImageCollider) {
					changeEasy = true;
					print ("easy");
				}  else {
					changeEasy = false;
				}
			}

			if (hit.collider.gameObject == buttonCollider) {
				//rayがヒットしたのがボタンだったら

				gaugeTime += Time.deltaTime * 0.01f;
				//ボタンと目線を合わせている時に、gaugeTimeを増加

				buttonGauge.rectTransform.localPosition = Vector3.Lerp (buttonGauge.rectTransform.localPosition, new Vector3 (0, 0, 1), gaugeTime);
			} else {
				gaugeTime = 0;
				buttonGauge.rectTransform.localPosition = firstButtonGaugePosition;
			}
				
			if (buttonGauge.rectTransform.localPosition.x > endPositionX) {
				//「ボタンのピンク色のゲージの位置が、endPositionより大きくなった場合trueを返す」

				ui.SetActive (false);

				if (changeHard) {
					stage.GetComponent<Renderer> ().material.color = Color.red;
				}
				if (changeNormal) {
					stage.GetComponent<Renderer> ().material.color = Color.yellow;
				}
				if (changeEasy) {
					stage.GetComponent<Renderer> ().material.color = Color.blue;
				}
			}
		}
	}
}
	