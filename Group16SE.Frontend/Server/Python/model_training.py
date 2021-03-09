import pandas as pd
from data_preparation import *
import numpy as np
import sys

example_dict = {'c_1': [1, 1, 0, 0, 0, 1, 0], 'c_2': [1, 0, 0, 1, 1, 1, 1], 'c_3': [0, 0, 0, 1, 0, 1, 1],
              'c_4': [1, 1, 1, 1, 0, 1, 0], 'c_5': [1, 0, 0, 1, 1, 0, 0], 'marks': [5, 7, 1, 9, 3, 8, 1]}

test_path = "/mongoDB_interaction/assighnment.json"

test_assighnment = "730aec0f-a23f-478e-9c94-787f9ab60526"


def dropNonCompleatedsections(dfs):
    """
    goes though each of the dataframes and finds each record that has been marked as completed
    returns a modified dict of the dataframes with the non complete records removed
    """
    newdf= {}
    for key in dfs:
        df = dfs[key]
        df = df.drop(df[df.finished != 1].index)
        del df['finished']
        newdf[key] = df

    return newdf

def makeDataFrame(JSONassighnment):
    DFs = createDataFrames(JSONassighnment)
    return DFs

def splitData(dataFrame, ratio):
    from sklearn.model_selection import train_test_split
    return train_test_split(dataFrame.drop(['mark'], axis = 'columbs'), dataFrame.mark, test_size= ratio)



def makeFreshRF(trees = 30):
    from sklearn.ensemble import RandomForestRegressor
    return RandomForestRegressor(n_estimators=trees)

def trainModel(model, train_x, train_y):
    return model.fit(train_x, train_y)

def queryModel(model, query_data_frame):
    return model.predict(query_data_frame)[0]



def createRFmodel(x_train, y_train, tree_num):

    model = makeFreshRF(tree_num)
    trainModel(model, x_train, list(y_train['mark']))

    return model

def spplitSectionLabeling(dfs):
    newdf= {}
    for key in dfs:
        df = dfs[key]

        feature_df, mark_df = df.drop(['mark'], axis=1), pd.DataFrame(df.mark)
        newdf[key] = {"features": feature_df, "labels": mark_df}

    return newdf


def createModlesForSections(dfs, tree_num, specificSection = False):
    newdf= {}
    for key in dfs:
        features = dfs[key]["features"]
        labels = dfs[key]["labels"]
    
        model = createRFmodel(features, labels, tree_num)

        if specificSection == False:
            newdf[key] = model
        elif key == specificSection:
            newdf[key] = model
        
    return newdf

def runOnSelf(features, model):
    predicted_labels = model.predict(features)
    return predicted_labels

def measureError(test_features, test_labels, model):
    from sklearn import metrics

    predicted_labels = runOnSelf(test_features, model)

    MAE = metrics.mean_absolute_error(test_labels, predicted_labels)
    RMSE = np.sqrt(metrics.mean_squared_error(test_labels, predicted_labels))

    return MAE, RMSE



def cmdLineMain():
    args = sys.argv
    AssignmentPath, SectionId, AttemptId = args[1], args[2], args[3]
    

    #for testing purposese, comment out below V
    #os.chdir("..")
    #AttemptId = "6017ffbc-d61c-46da-a8c5-21ef6d5a3f21"
    #SectionId = "68fb5c46-4746-4b66-b0d2-6f11f950f915"
    #AssignmentPath = "local_storage_interaction/assighnment.json"
    ##comment out above^


    with open(AssignmentPath) as f:
        content  = json.load(f)

    # preparing dataframes
    dfs = createDataFrames(content)
    q_df = createDataFrames(content,  atttemptID = AttemptId)

    # gets rid of sections in attepts that ahve not been marked as compleated
    filtered_dfs = dropNonCompleatedsections(dfs)

    # splits the feature and label parts
    split_dfs = spplitSectionLabeling(filtered_dfs)
    q_split_df = spplitSectionLabeling(q_df)["68fb5c46-4746-4b66-b0d2-6f11f950f915"]['features'].drop(["finished"], axis=1)





    # create models for each section
    tree_num = 30
    model = createModlesForSections(split_dfs, tree_num=tree_num, specificSection=SectionId)

    q_answer = queryModel(model[SectionId], q_split_df)


    acc = measureError(split_dfs[SectionId]["features"], split_dfs[SectionId]["labels"], model[SectionId])

    return q_answer, acc


if __name__ == '__main__':
    print(cmdLineMain())
#testing with other data sets
#digits = load_digits()
#print(dir(digits))