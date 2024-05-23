using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

public class UIReader_ImageFinding : UI_Reader
{
    // UXML
    VisualElement imageGround;
    //VisualElement evidenceExplanation;

    // Template
    VisualTreeAsset ux_imageGround;
    VisualTreeAsset ux_imageEvidence;
    VisualTreeAsset ux_evidenceExplanation;

    private void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        base.OnEnable();

        UXML_Load();
        Template_Load();
    }

    private void UXML_Load()
    {
        imageGround = root.Q<VisualElement>("ImageFinding");
        //evidenceExplanation = root.Q<VisualElement>("EvidenceDescript");
    }

    private void Template_Load()
    {
        ux_imageGround = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Evidence\\ImageGround.uxml");
        ux_imageEvidence = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Evidence\\Evidence.uxml");
        ux_evidenceExplanation = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets\\UI Toolkit\\Prefab\\Evidence\\EvidenceDescript.uxml");
    }

    public void EventImage(VisualElement file)
    {
        // Image�� ��
        foreach (ImageB image in imageManager.images)
        {
            // �ش� �̹����� ã�Ҵٸ� ��� ����
            if (image.name == file.Q<Label>("FileName").text)
            {
                if (image.isOpen)
                    imageGround.Remove(imageGround.Q<VisualElement>(image.name));
                else
                {
                    VisualElement imageBackground = RemoveContainer(ux_imageGround.Instantiate());
                    imageBackground.name = image.name;
                    imageBackground.style.backgroundImage = new StyleBackground(image.image);
                    imageGround.Add(imageBackground);

                    // �ڽ� �ܼ����� ����
                    // �̸����� ã���ְ�
                    foreach (string evid in image.pngName)
                    {
                        // �ش� �ܼ��� ã�Ҵٸ�
                        foreach (ImagePng png in imageManager.pngs)
                        {
                            if (evid == png.name)
                            {
                                // ����
                                VisualElement evidence = null;
                                // �߿��ϴٸ�
                                if (png.importance)
                                {
                                    // �޸������� ǥ��
                                    evidence = RemoveContainer(ux_imageEvidence.Instantiate());
                                    evidence.Q<Button>("EvidenceImage").style.backgroundImage = new StyleBackground(png.image);
                                    evidence.Q<VisualElement>("Descripte").Q<Label>("EvidenceName").text = png.name;
                                    evidence.Q<VisualElement>("Descripte").Q<Label>("Memo").text = png.memo;
                                    evidence.Q<Button>("EvidenceImage").clicked += (() =>
                                    {
                                        VisualElement description = evidence.Q<VisualElement>("Descripte");
                                        description.style.display = DisplayStyle.Flex;

                                        if (png.isOpen == false)
                                        {
                                            png.isOpen = true;
                                            Debug.Log(png.name + " " + image.name);
                                            fileSystem.AddFile(FileType.IMAGE, png.name, image.name);
                                        }
                                    });
                                }
                                // �ƴ϶��
                                else
                                {
                                    // �Ʒ� �۷θ� ǥ��
                                    evidence = RemoveContainer(ux_imageEvidence.Instantiate());
                                    evidence.Q<Button>("EvidenceImage").style.backgroundImage = new StyleBackground(png.image);
                                    evidence.Q<Button>("EvidenceImage").clicked += (() =>
                                    {
                                        VisualElement evidenceDescription = RemoveContainer(ux_evidenceExplanation.Instantiate());
                                        imageGround.Add(evidenceDescription);
                                        DoText(evidenceDescription.Q<Label>("Text"), png.memo, 3f, 
                                            () => { imageGround.Remove(evidenceDescription); });
                                    });
                                }

                                //�ܼ� ��ġ ����
                                evidence.style.position = Position.Absolute;
                                evidence.style.left = png.pos.x;
                                evidence.style.top = png.pos.y;
                                // �ܼ��� �̹����� �߰�
                                imageBackground.Add(evidence);
                            }
                        }
                    }
                }

                image.isOpen = !image.isOpen;
            }
        }

        // Png�� ��
        foreach (ImagePng png in imageManager.pngs)
        {
            if (png.name == file.Q<Label>("FileName").text)
            {
                fileSystem.OpenImage(png.name, png.image);
            }
        }
    }
}
        
            //foreach (ImageDefualt image in imageManager.images)
            //{
            //    if (image.name == file.Q<Label>("FileName").text)
            //    {
            //        // �̹����� ��. ä���̵� ���� ����
            //        // for�� ���鼭? �ڽ� ���? - evidence ���ø��� ������!
            //        // ������ ��. ���࿡ �߿��ѰŸ� ���ø� ����, �ƴϸ� �׳� �̹�����...

            //        // visualelement�� ��� ���?
            //        VisualElement imageBackground = RemoveContainer(ux_imageGround.Instantiate());
            //        // �̹����� ������.
            //        imageBackground.style.backgroundImage = new StyleBackground(image.image);
            //        // �̹����� �߰�
            //        imageGround.Add(imageBackground);

            //        // �ش� �̹����� �ܼ��� ��ġ
            //        foreach (ImagePng evid in image.)
            //        {
            //            VisualElement evidence = null;
            //            // �߿��� �ܼ����
            //            if (evid.importance)
            //            {
            //                // ���� �� �̹��� ����
            //                evidence = RemoveContainer(ux_imageEvidence.Instantiate());
            //                evidence.Q<Button>("EvidenceImage").style.backgroundImage = new StyleBackground(evid.evidenceImage);
            //                // �ܼ� �ڷ� �߰�
            //                evidence.Q<VisualElement>("Descripte").Q<Label>("EvidenceName").text = evid.evidenceName;
            //                evidence.Q<VisualElement>("Descripte").Q<Label>("Memo").text = evid.evidenceMemo;
            //                // �̺�Ʈ ���� - �޸��� ������ �̺�Ʈ
            //                evidence.Q<Button>("EvidenceImage").clicked += (() =>
            //                {
            //                    VisualElement description = evidence.Q<VisualElement>("Descripte");
            //                    description.style.display = DisplayStyle.Flex;
            //                });
            //            }
            //            else
            //            {
            //                // ���� �� �̹��� ����
            //                evidence = RemoveContainer(ux_imageGround.Instantiate());
            //                evidence.style.backgroundImage = new StyleBackground(evid.evidenceImage);
            //                // �̺�Ʈ ���� - �Ʒ� ������ ������ �̺�Ʈ
            //                evidence.Q<Button>("EvidenceImage").clicked += (() =>
            //                {
            //                    // ���� �ʿ���...
            //                    evidenceExplanation.style.display = DisplayStyle.Flex;
            //                    DoText(evidenceExplanation.Q<Label>("Text"), evid.evidenceMemo,
            //                        3f, () => { });
            //                });
            //            }

            //            // �ܼ� ��ġ ����
            //            evidence.style.position = Position.Absolute;
            //            evidence.style.left = evid.evidencePos.x;
            //            evidence.style.top = evid.evidencePos.y;
            //            // �ܼ��� �̹����� �߰�
            //            imageBackground.Add(evidence);
            //        }
            //    }
            //}