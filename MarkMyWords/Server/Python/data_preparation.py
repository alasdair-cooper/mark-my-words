from store import *
import pandas as pd

DATABASE = 'test_main'
COLLECTION1 = 'Attempts'
COLLECTION2 = 'Assignments'
CLUSTER = "mongodb+srv://dev_user:Durham@cluster0.wu3em.mongodb.net/<dbname>?retryWrites=true&w=majority"

test_path = "C:/GIT/S_enge/backend/S_Eng-group-16-backend/mongoDB_interaction/assighnment.json"

test_assighnment = "730aec0f-a23f-478e-9c94-787f9ab60526"

def getAssighment(assighnment_name):
    #cahnges the directory to the root direcotry( be carful if moving files around!)
    os.chdir("..")
    return assignment().getOne(assighnment_name)

def getAssighmentViaPath(path):
    #cahnges the directory to the root direcotry( be carful if moving files around!)
    os.chdir("..")
    return fileMangagement().openJSONDirect(path)



class dataFrormatting:

    def __init__(self, content):

        self.rawJson = content

    def createDictOfMarkings(self):
        """
        creates a dictionary of where each comment presence will be in the feature vector
        dict has the $ID of the comment/point as its key and its position in the feature vector as the value
        """
        rawJson = self.rawJson
        section_dicts = {}

        comment_bank = rawJson["SectionCommentBanks"]
        point_bank = rawJson["SectionPointBanks"]

        section_names = list(comment_bank)
        point_names = list(comment_bank)

        for sec_name in section_names:
            vector_dict = {}
            feature_vector_place = 0
            for comment in rawJson["SectionCommentBanks"][sec_name]:
                vector_dict[comment["CommentId"]] = feature_vector_place
                feature_vector_place += 1

            for point in rawJson["SectionPointBanks"][sec_name]:
                vector_dict[point["PointId"]] = feature_vector_place
                feature_vector_place += 1

            section_dicts[sec_name] = vector_dict

        return section_dicts


    def attempts2Vectors(self, assighnment, specific = None):

        section_dicts = self.createDictOfMarkings()
        section_names = list(assighnment["SectionCommentBanks"])

        attempt_vecs = []

        for attempt in assighnment["Attempts"]:
            if attempt["AttemptId"] == specific or specific == None:
                sec_vectors = []
                section_num = 0
                for section in attempt["Sections"]:

                    section_vec = [0 for i in range(len(section_dicts[section_names[section_num]]))]

                    for comment in section["Comments"]:
                        comment_id = str(comment["CommentId"])
                        section_vec[section_dicts[section["SectionID"]][comment_id]] = 1


                    """ untill alidair fixes the point ids
                    for point in section["Points"]["$values"]:
                        point_id = str(point["PointId"])
                        point_val = point["Value"]
                    
                        if point_val == True:
                            point_val = 1
                        if point_val == False:
                            point_val = 0
                        section_vec[section_dicts[section["SectionID"]][point_id]] = point_val
                    """

                    section_vec.append(section["GivenMark"])
                    if attempt["Completed"] == True:
                        section_vec.append(1)
                    else:
                        section_vec.append(0)


                    sec_vectors.append(section_vec)

                attempt_vecs.append(sec_vectors)

        return attempt_vecs


    def dataframeFormat(self, attempt_vecs, assighnment):
        """
        takes each attempts section vectors and puts formats them as a pandas data frame
        """
        section_names = list(self.createDictOfMarkings())
        section_dfs = {}


        for n_i in range(len(section_names)):
            df = pd.DataFrame(list(pd.DataFrame(attempt_vecs)[n_i]))
            cols =[f"x_{i}" for i in range(len(df.columns)-2)]
            cols.append("mark")
            cols.append("finished")
            df.columns = cols
            section_dfs[section_names[n_i]] = df

        return section_dfs




#get the JSON from of test_assighment and then parse it to get the feature vectors

def createDataFrames(content , atttemptID = None):
    """
    second last in each record in each section is section marked
    last in each record is the mark for each section
    """

    workspace = dataFrormatting(content)

    if atttemptID != None:
        selected = workspace.attempts2Vectors(content, atttemptID)
        selected_df = workspace.dataframeFormat(selected, content)
        return selected_df

    assighment_JSON = workspace.rawJson
    attempt_vectors = workspace.attempts2Vectors(assighment_JSON)
    section_dataframes = workspace.dataframeFormat(attempt_vectors, assighment_JSON)

    return section_dataframes




if __name__ == '__main__':
    frames = createDataFrames(test_assighnment)
    print(frames)