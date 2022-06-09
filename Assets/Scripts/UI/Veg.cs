using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Veg : MonoBehaviour {
    public float blowImpulse;
    private Rigidbody2D rb;

    public void OnTapped() {
        StartCoroutine(KaBoom());
        GetComponent<Button>().interactable = false;
        rb = GetComponent<Rigidbody2D>();
    }

    private IEnumerator KaBoom() {
        float scale = 1;

        while (scale < 1.5f) {
            scale *= 1.05f;

            gameObject.transform.localScale = Vector3.one * scale;
            yield return new WaitForFixedUpdate();
        }

        gameObject.GetComponent<Image>().enabled = false;
        gameObject.transform.localScale = Vector3.one * scale * 5;
        rb.AddForce(Vector2.up * blowImpulse * rb.mass, ForceMode2D.Impulse);
        yield return new WaitForFixedUpdate();
        Destroy(gameObject);
    }
}