# Video translation

Video translation client tool and API sample code.

This is advanced version for video translation API.

This version support more features which is enabled before releasing to Azure doc for public preview.

# Advanced arguments

## For createTranslationAndIterationAndWaitUntilTerminated
   | Argument | Is Required | Supported Values Sample | Description |
   | -------- | -------- | ---------------- | ----------- |
   | --videoFileAzureBlobUrl  | False | URL | Translate video file, videoFileAzureBlobUrl and audioFileAzureBlobUrl are conflict, only one of them are required. |
   | --audioFileAzureBlobUrl  | False | URL | Translate audio file, videoFileAzureBlobUrl and audioFileAzureBlobUrl are conflict, only one of them are required. |
   | --enableLipSync  | False | True | Enable lip sync. |
   | --ttsCustomLexiconFileUrl  | False | URL | Translate with TTS custom lexicon for TTS synthesis, proivde the custom lexicon file with URL, ttsCustomLexiconFileUrl and ttsCustomLexiconFileIdInAudioContentCreation are conflict, only one of them are requied. |
   | --ttsCustomLexiconFileIdInAudioContentCreation  | False | Guid | Translate with TTS custom lexicon for TTS synthesis, proivde the custom lexicon file with file ID in Audio Content Creation, ttsCustomLexiconFileUrl and ttsCustomLexiconFileIdInAudioContentCreation are conflict, only one of them are requied. |

## For createIterationAndWaitUntilTerminated
   | Argument | Is Required | Supported Values Sample | Description |
   | -------- | -------- | ---------------- | ----------- |
   | --audioFileAzureBlobUrl  | False | URL | Translate audio file. |
   | --enableLipSync  | False | True | Enable lip sync. |
   | --ttsCustomLexiconFileUrl  | False | URL | Translate with TTS custom lexicon for pronunciation lexicon, ttsCustomLexiconFileUrl and ttsCustomLexiconFileIdInAudioContentCreation are conflict, only one of them are requied. |
   | --ttsCustomLexiconFileIdInAudioContentCreation  | False | Guid | Translate with TTS custom lexicon for TTS synthesis, proivde the custom lexicon file with file ID in Audio Content Creation, ttsCustomLexiconFileUrl and ttsCustomLexiconFileIdInAudioContentCreation are conflict, only one of them are requied. |

## For createTranslation
   | Argument | Is Required | Supported Values Sample | Description |
   | -------- | -------- | ---------------- | ----------- |
   | --videoFileAzureBlobUrl  | False | URL | Translate video file, videoFileAzureBlobUrl and audioFileAzureBlobUrl are conflict, only one of them are required. |
   | --audioFileAzureBlobUrl  | False | URL | Translate audio file, videoFileAzureBlobUrl and audioFileAzureBlobUrl are conflict, only one of them are required. |
   | --enableLipSync  | False | True | Enable lip sync. |

# For general version
	https://github.com/Azure-Samples/cognitive-services-speech-sdk/tree/master/samples/video-translation/csharp