using Treegen;
using UnityEngine;

public class CloneOnPress : MonoBehaviour
{
    public GameObject bulleLandPrefab;  // Le prefab contenant l'arbre et autres objets
    public GameObject arbreOriginel;
    private TreegenTreeGenerator treegenTreeGenerator;  // Générateur d'arbre du prefab
    private float savedTreeNoiseForce;
    private float savedTrunkThickness;
    private Vector3 savedLeavesScale;

    private float resetTimer = 0f;
    private bool isResetPending = false;

    private int cloneCount = 1;  // Compteur pour savoir quel clone on crée

    // Position de l'arbre originel que tu veux restaurer après réinitialisation
    private Vector3 originalPosition = new Vector3(-0.368030012f, -2.70000005f, 0.588739991f);

    void Start()
    {
        if (bulleLandPrefab != null)
        {
            // Recherche TreegenTreeGenerator dans l'arbre originel
            treegenTreeGenerator = arbreOriginel.GetComponent<TreegenTreeGenerator>();

            if (treegenTreeGenerator != null)
            {
                // Sauvegarde les propriétés initiales de TreegenTreeGenerator
                savedTreeNoiseForce = treegenTreeGenerator.TreeNoiseForce;
                savedTrunkThickness = treegenTreeGenerator.TrunkThickness.keys[1].value;
                savedLeavesScale = treegenTreeGenerator.LeavesScale;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            CloneAndMove();
            isResetPending = true;
            resetTimer = 3f;
        }

        if (isResetPending)
        {
            resetTimer -= Time.deltaTime;

            if (resetTimer <= 0f)
            {
                // Réinitialiser l'arbre originel après la création du clone
                ResetArbreOriginel();
                isResetPending = false;
            }
        }
    }

    void CloneAndMove()
    {
        // Cloner le prefab entier
        GameObject clone = Instantiate(bulleLandPrefab);

        // Positionner le clone en fonction du nombre de clones
        float offset = cloneCount * 35f;  // Décaler chaque clone de 20 unités sur l'axe X (exemple négatif)
        clone.transform.position = new Vector3(offset, 0f, offset);  // Positionner le clone

        // Incrémenter le compteur de clones
        cloneCount++;

        // Si le clone contient un TreegenTreeGenerator, appliquer les réglages
        TreegenTreeGenerator cloneTreegen = clone.GetComponent<TreegenTreeGenerator>();

        if (cloneTreegen != null)
        {
            // Appliquer les propriétés sauvegardées au clone
            cloneTreegen.TreeNoiseForce = savedTreeNoiseForce;
            cloneTreegen.TrunkThickness.keys[1].value = Mathf.Clamp(savedTrunkThickness, 0.1f, 2.0f);
            cloneTreegen.LeavesScale = savedLeavesScale;

            // Régénérer l'arbre du clone avec les mêmes réglages
            cloneTreegen.NewGen();
        }
    }

    void ResetArbreOriginel()
    {
        if (treegenTreeGenerator != null)
        {

            // Réinitialiser les propriétés de TreegenTreeGenerator avec les valeurs sauvegardées
            treegenTreeGenerator.TreeNoiseForce = savedTreeNoiseForce;
            treegenTreeGenerator.TrunkThickness.keys[1].value = Mathf.Clamp(savedTrunkThickness, 0.1f, 2.0f);
            treegenTreeGenerator.LeavesScale = savedLeavesScale;

            // Regénérer l'arbre avec les valeurs restaurées
            treegenTreeGenerator.NewGen();

        }

        // Réinitialiser la position de l'arbre originel
        arbreOriginel.transform.position = originalPosition;

        // Activer l'arbre originel si jamais il a été désactivé
        arbreOriginel.SetActive(true);
    }
}
