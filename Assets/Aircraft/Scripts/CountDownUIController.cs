using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Aircraft
{

    public class CountDownUIController : MonoBehaviour
    {
        // Start is called before the first frame update

        public TextMeshProUGUI countdownText;

        public IEnumerator StartCountdown()
        {
            countdownText.text = "3";
            yield return new WaitForSeconds(1f);
            countdownText.text = string.Empty;
            yield return new WaitForSeconds(0.5f);

            countdownText.text = "2";
            yield return new WaitForSeconds(1f);
            countdownText.text = string.Empty;
            yield return new WaitForSeconds(0.5f);

            countdownText.text = "1";
            yield return new WaitForSeconds(1f);
            countdownText.text = string.Empty;
            yield return new WaitForSeconds(0.5f);

            countdownText.text = "GO";
            yield return new WaitForSeconds(1f);
            countdownText.text = string.Empty;
            yield return new WaitForSeconds(0.5f);

            this.gameObject.SetActive(false);


        }
    }

}