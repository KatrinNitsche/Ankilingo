using Assets.Scripts.Dtos;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Assets.Scripts.Objects;

namespace Assets.Scripts
{
    public class CourseService : MonoBehaviour
    {
        private string courseURL = "http://localhost:5123/Course";
        private string sectionURL = "http://localhost:5123/Section";
        private TokenResponse token;
        private string username;
        private string password;

        [Serializable]
        private class TokenResponse
        {
            public string token;
        }

        [Serializable]
        private class LoginData
        {
            public string username;
            public string password;
        }

        [Serializable]
        public class CourseList
        {
            public CourseObject[] courses;
        }

        [Serializable]
        public class SectionList
        {
            public SectionObject[] sections;
        }

        public void SetToken(string username, string password)
        {
            this.username = username;
            this.password = username;
            StartCoroutine(RequestToken(username, password, token =>
            {
                if (!string.IsNullOrEmpty(token))
                {
                    this.token = new TokenResponse { token = token };
                }
                else
                {
                    Debug.LogError("Failed to retrieve token.");
                }
            }));
        }

        public IEnumerator GetCourses(Action<CourseObject[]> onCoursesReceived)
        {
            if (token == null || string.IsNullOrEmpty(token.token))
            {
                Debug.LogError($"{nameof(GetCourses)}: Token is null or empty. Cannot fetch courses.");
                yield break;
            }

            using (UnityWebRequest webRequest = UnityWebRequest.Get(courseURL))
            {
                webRequest.SetRequestHeader("Authorization", "Bearer " + token.token);
                webRequest.SetRequestHeader("Accept", "application/json");
                webRequest.SetRequestHeader("Content-Type", "application/json");

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        string rawJson = webRequest.downloadHandler.text;         
                        string wrappedJson = "{ \"courses\": " + rawJson + "}";
                        var courseList = JsonUtility.FromJson<CourseList>(wrappedJson);

                        onCoursesReceived?.Invoke(courseList?.courses);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"{nameof(GetCourses)}: Failed to parse courses: {ex.Message}");
                        onCoursesReceived?.Invoke(null);
                    }
                }
                else
                {
                    Debug.LogError($"{nameof(GetCourses)}: Error: {webRequest.error}");
                    onCoursesReceived?.Invoke(null);
                }
            }
        }

        public IEnumerator GetSections(int courseId, Action<SectionObject[]> onSectionsReceived)
        {
            if (token == null || string.IsNullOrEmpty(token.token))
            {
                Debug.LogError($"{nameof(GetCourses)}: Token is null or empty. Cannot fetch courses.");
                yield break;
            }
                      
            string url = $"{sectionURL}/{courseId.ToString()}";           
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) 
            {
                webRequest.SetRequestHeader("Authorization", "Bearer " + token.token);
                webRequest.SetRequestHeader("Accept", "application/json");
                webRequest.SetRequestHeader("Content-Type", "application/json");

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        string rawJson = webRequest.downloadHandler.text;
                        string wrappedJson = "{ \"sections\": " + rawJson + "}";
                        var list = JsonUtility.FromJson<SectionList>(wrappedJson);

                        onSectionsReceived?.Invoke(list?.sections);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"{nameof(GetSections)}: Failed to parse sections: {ex.Message}");
                        onSectionsReceived?.Invoke(null);
                    }
                }
                else
                {
                    Debug.LogError($"{nameof(GetSections)}: Error: {webRequest.error}");
                    onSectionsReceived?.Invoke(null);
                }
            }
        }


        // Add a new course
        public IEnumerator AddCourse(CourseObject course, Action<bool> onComplete)
        {
            if (token == null || string.IsNullOrEmpty(token.token))
            {
                Debug.LogError($"{nameof(AddCourse)}: Token is null or empty. Cannot add course.");
                onComplete?.Invoke(false);
                yield break;
            }

            var courseToAdd = new AddCourseDto()
            {
                description = course.description,
                icon = course.icon,
                name = course.name
            };

            string jsonData = JsonConvert.SerializeObject(courseToAdd);

            using (UnityWebRequest webRequest = new UnityWebRequest(courseURL, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Authorization", "Bearer " + token.token);
                webRequest.SetRequestHeader("Content-Type", "application/json");

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    onComplete?.Invoke(true);
                }
                else
                {
                    Debug.LogError($"{nameof(AddCourse)}: Error: {webRequest.error}"); // AddCourse: Error: HTTP/1.1 400 Bad Request
                    onComplete?.Invoke(false);
                }
            }
        }

        // Fix for CS1503: Argument 1: cannot convert from 'System.Guid' to 'string'
        // The issue occurs because `course.id` is of type `Guid`, but the method expects a `string`.
        // To fix this, we need to convert the `Guid` to a string using `course.id.ToString()`.

        public IEnumerator UpdateCourse(CourseObject course, Action<bool> onComplete)
        {
            if (token == null || string.IsNullOrEmpty(token.token))
            {
                Debug.LogError($"{nameof(UpdateCourse)}: Token is null or empty. Cannot update course.");
                onComplete?.Invoke(false);
                yield break;
            }

            if (course.id == 0) 
            {
                Debug.LogError($"{nameof(UpdateCourse)}: Course ID is empty.");
                onComplete?.Invoke(false);
                yield break;
            }

            var courseToUpdate = new EditCourseDto()
            {
                description = course.description,
                icon = course.icon,
                name = course.name,
                id = course.id
            };

            string url = $"{courseURL}/{course.id.ToString()}";
            string jsonData = JsonConvert.SerializeObject(courseToUpdate);

            using (UnityWebRequest webRequest = new UnityWebRequest(url, "PUT"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Authorization", "Bearer " + token.token);
                webRequest.SetRequestHeader("Content-Type", "application/json");

                yield return webRequest.SendWebRequest(); // UpdateCourse: Error: HTTP/1.1 400 Bad Request

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    onComplete?.Invoke(true);
                }
                else
                {
                    Debug.LogError($"{nameof(UpdateCourse)}: Error: {webRequest.error}");
                    onComplete?.Invoke(false);
                }
            }
        }

        // Get a single course by ID
        public IEnumerator GetCourse(string courseId, Action<CourseObject> onCourseReceived)
        {
            if (token == null || string.IsNullOrEmpty(token.token))
            {
                Debug.LogError($"{nameof(GetCourse)}: Token is null or empty. Cannot fetch course.");
                onCourseReceived?.Invoke(null);
                yield break;
            }

            if (string.IsNullOrEmpty(courseId))
            {
                Debug.LogError($"{nameof(GetCourse)}: Course ID is null or empty.");
                onCourseReceived?.Invoke(null);
                yield break;
            }

            string url = $"{courseURL}/{courseId}";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                webRequest.SetRequestHeader("Authorization", "Bearer " + token.token);
                webRequest.SetRequestHeader("Accept", "application/json");
                webRequest.SetRequestHeader("Content-Type", "application/json");

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        var course = JsonUtility.FromJson<CourseObject>(webRequest.downloadHandler.text);
                        onCourseReceived?.Invoke(course);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"{nameof(GetCourse)}: Failed to parse course: {ex.Message}");
                        onCourseReceived?.Invoke(null);
                    }
                }
                else
                {
                    Debug.LogError($"{nameof(GetCourse)}: Error: {webRequest.error}");
                    onCourseReceived?.Invoke(null);
                }
            }
        }

        // Fix for CS1503: Argument 1: cannot convert from 'System.Guid' to 'string'
        // The issue occurs because `courseId` is of type `Guid`, but the method expects a `string`.
        // To fix this, we need to convert the `Guid` to a string using `courseId.ToString()`.

        public IEnumerator RemoveCourse(int courseId, Action<bool> onComplete)
        {
            if (token == null || string.IsNullOrEmpty(token.token))
            {
                Debug.LogError($"{nameof(RemoveCourse)}: Token is null or empty. Cannot remove course.");
                onComplete?.Invoke(false);
                yield break;
            }

            if (courseId == 0) 
            {
                Debug.LogError($"{nameof(RemoveCourse)}: Course ID is empty.");
                onComplete?.Invoke(false);
                yield break;
            }

            string url = $"{courseURL}/{courseId.ToString()}"; // Convert Guid to string here.

            using (UnityWebRequest webRequest = UnityWebRequest.Delete(url))
            {
                webRequest.SetRequestHeader("Authorization", "Bearer " + token.token);
                webRequest.SetRequestHeader("Content-Type", "application/json");

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    onComplete?.Invoke(true);
                }
                else
                {
                    Debug.LogError($"{nameof(RemoveCourse)}: Error: {webRequest.error}");
                    onComplete?.Invoke(false);
                }
            }
        }

        private IEnumerator RequestToken(string username, string password, Action<string> onTokenReceived)
        {
            string loginUrl = "http://localhost:5123/api/Auth/token";
            var loginData = new LoginData { username = username, password = password };
            string jsonData = JsonUtility.ToJson(loginData);

            using (UnityWebRequest webRequest = new UnityWebRequest(loginUrl, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonUtility.FromJson<TokenResponse>(webRequest.downloadHandler.text);
                    onTokenReceived?.Invoke(response.token);
                }
                else
                {
                    Debug.LogError("Token request failed: " + webRequest.error);
                    onTokenReceived?.Invoke(null);
                }
            }
        }
    }
}
