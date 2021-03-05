import pandas as pd
from data_preparation import createDataFrames, getAssighment
import numpy as np

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
    return model.predict(query_data_frame).max



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


def createModlesForSections(dfs, tree_num):
    newdf= {}
    for key in dfs:
        features = dfs[key]["features"]
        labels = dfs[key]["labels"]
    
        model = createRFmodel(features, labels, tree_num)
        
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


def main():
    #preparing dataframes
    dfs = createDataFrames(test_assighnment)
    #gets rid of sections in attepts that ahve not been marked as compleated
    filtered_dfs = dropNonCompleatedsections(dfs)
    #splits the feature and label parts
    split_dfs = spplitSectionLabeling(filtered_dfs)
    
    
    #create models for each section
    tree_num = 30
    models = createModlesForSections(split_dfs, tree_num=tree_num)

if __name__ == '__main__':
    main()
#testing with other data sets
#digits = load_digits()
#print(dir(digits))